// MarkovWebApp/Logic/MarkovGenerator.cs

using MarkovWebApp.Services;
using MarkovWebApp.Data.Models;

namespace MarkovWebApp.Logic
{
    public class MarkovGenerator
    {
        private readonly MarkovDbService _dbService;
        private readonly Random _random = new();
        private const string KeyDelimiter = "||";

        public MarkovGenerator(MarkovDbService dbService)
        {
            _dbService = dbService;
        }

        public async IAsyncEnumerable<string> GenerateTextAsync(string seedPhrase, int length, int maxOrder = 2)
        {
            var words = seedPhrase.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            // Başlangıç kelimelerini yield et
            foreach (var word in words)
            {
                yield return word;
            }

            for (int i = 0; i < length; i++)
            {
                string? nextWord = null;

                // KATZ'S BACK-OFF STRATEJİSİ
                for (int order = maxOrder; order >= 1; order--)
                {
                    if (words.Count < order) continue;

                    var keyWords = words.TakeLast(order);
                    var key = string.Join(KeyDelimiter, keyWords);

                    var possibilities = await _dbService.GetPossibleNextWordsAsync(key);

                    if (possibilities.Any())
                    {
                        // Olasılıkları bulduk, ağırlıklı seçim yap ve döngüden çık
                        nextWord = WeightedRandomSelect(possibilities);
                        break;
                    }
                    // Sonuç yoksa, bir alt dereceye (daha kısa anahtara) düş ve tekrar dene (back-off)
                }

                if (string.IsNullOrEmpty(nextWord))
                {
                    // Zincir tamamen koptu, daha fazla üretemiyoruz
                    break;
                }

                words.Add(nextWord);
                yield return nextWord; // Kelimeyi anında akıt (SignalR için hazırlık)
            }
        }

        // Ağırlıklı rastgele seçim metodu
        private string? WeightedRandomSelect(List<Ngram> possibilities)
        {
            if (possibilities == null || !possibilities.Any())
            {
                return null;
            }

            int totalWeight = possibilities.Sum(p => p.Count);

            // SAVUNMA HATTI: Eğer toplam ağırlık pozitif değilse,
            // çekiliş yapmak anlamsız ve tehlikelidir. Sadece rastgele birini seç ve devam et.
            if (totalWeight <= 0)
            {
                return possibilities[_random.Next(possibilities.Count)].NextWord;
            }

            int randomNum = _random.Next(1, totalWeight + 1);

            int cumulativeWeight = 0;
            foreach (var possibility in possibilities)
            {
                cumulativeWeight += possibility.Count;
                if (randomNum <= cumulativeWeight)
                {
                    return possibility.NextWord;
                }
            }

            return null; // Normalde buraya asla ulaşmamalı, ama bir sigorta olarak kalmalı.
        }

    }
}