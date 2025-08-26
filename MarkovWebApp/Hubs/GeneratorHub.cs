// MarkovWebApp/Hubs/GeneratorHub.cs

using Microsoft.AspNetCore.SignalR;
using MarkovWebApp.Logic;
using System.Text;

namespace MarkovWebApp.Hubs
{
    public class GeneratorHub : Hub
    {
        private readonly MarkovGenerator _generator;
        private readonly ILogger<GeneratorHub> _logger; // 1. ILogger'ı ekle

        // 2. Constructor'a ILogger'ı enjekte et
        public GeneratorHub(MarkovGenerator generator, ILogger<GeneratorHub> logger)
        {
            _generator = generator;
            _logger = logger;
        }

        public async Task GenerateStream(string seedPhrase, int length = 100)
        {
            // 3. Tüm metodu bir try-catch bloğuna al
            try
            {
                var postProcessor = new TextPostProcessor();
                
                _logger.LogInformation("Streaming started for seed phrase: {SeedPhrase}", seedPhrase);

                await foreach (var word in _generator.GenerateTextAsync(seedPhrase, length))
                {
                    string cleanedWord = postProcessor.ProcessNextWord(word);
                    if (!string.IsNullOrEmpty(cleanedWord))
                    {
                        await Clients.Caller.SendAsync("ReceiveWord", cleanedWord);
                    }
                }

                string? finalFlush = postProcessor.Flush();
                if (!string.IsNullOrEmpty(finalFlush))
                {
                    await Clients.Caller.SendAsync("ReceiveWord", finalFlush);
                }

                await Clients.Caller.SendAsync("StreamFinished");
                _logger.LogInformation("Streaming finished successfully for seed phrase: {SeedPhrase}", seedPhrase);
            }
            catch (Exception ex)
            {
                // 4. Hata olursa, hem terminale logla hem de istemciye haber ver.
                _logger.LogError(ex, "An error occurred in GenerateStream for seed phrase {SeedPhrase}", seedPhrase);
                
                // İstemciye de hatanın detayını gönderelim (EnableDetailedErrors sayesinde)
                // Bu satır yerine, istemciye özel bir hata mesajı da gönderebiliriz.
                // await Clients.Caller.SendAsync("StreamError", "An unexpected error occurred on the server.");
                
                // Hatayı yeniden fırlatarak SignalR'ın kendi mekanizmasının çalışmasını sağlıyoruz.
                throw;
            }
        }
    }

    // GÖRGÜ KURALLARI SINIFI (GELİŞTİRİLMİŞ VERSİYON)
    public class TextPostProcessor
    {
        private bool _isInsideQuote = false;
        private string? _lastToken = null;

        public string ProcessNextWord(string word)
        {
            // Varsayılan olarak kelimenin başına bir boşluk ekleyeceğiz.
            bool addSpace = true;

            // Kural 1: Bu, üretilen ilk kelimeyse, başına boşluk ekleme.
            if (_lastToken == null)
            {
                addSpace = false;
            }
            // Kural 2: Mevcut kelime standart bir noktalama işaretleri ise, başına boşluk ekleme.
            // (Örn: "kelime" + "," -> "kelime,")
            else if (word.Length == 1 && ".?!,;'".Contains(word))
            {
                addSpace = false;
            }
            // Kural 3: Bir önceki kelime, bir cümlenin başlangıcını belirten AÇILIŞ tırnağı ise,
            // ondan sonra gelen kelimenin başına boşluk ekleme.
            // Bir önceki tırnağın "açılış" tırnağı olduğunu, _isInsideQuote durumunun o anda 'true' olmasından anlıyoruz.
            else if (_lastToken == "\"" && _isInsideQuote)
            // başına boşluk ekleme.
            {
                addSpace = false;
            } // Kural 4: Mevcut kelime bir cümlenin sonunu belirten KAPANIŞ tırnağı ise,
            // ondan önce gelen kelimenin başına boşluk ekleme.

            // Karar verilen boşluğu uygula.
            string output = addSpace ? " " + word : word;

            // Kararları verdikten sonra, mevcut kelimeye göre durumu GÜNCELLE.
            if (word == "\"")
            {
                _isInsideQuote = !_isInsideQuote;
            }
            _lastToken = word;

            return output;
        }

        // Akış sonunda açık kalmış bir tırnak varsa kapat.
        public string? Flush()
        {
            if (_isInsideQuote)
            {
                _isInsideQuote = false;
                // Kapanış tırnağından önce boşluk olmaz.
                return "\"";
            }
            return null;
        }
    }


}