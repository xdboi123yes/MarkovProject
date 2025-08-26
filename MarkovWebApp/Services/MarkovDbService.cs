// MarkovWebApp/Services/MarkovDbService.cs

using Microsoft.EntityFrameworkCore;
using MarkovWebApp.Data;
using MarkovWebApp.Data.Models;

namespace MarkovWebApp.Services
{
    public class MarkovDbService
    {
        private readonly IDbContextFactory<MarkovDbContext> _contextFactory;
        private const string KeyDelimiter = "||";

        public MarkovDbService(IDbContextFactory<MarkovDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Metni işleyip veritabanını eğiten ana metot
        public async Task TrainAsync(string text, int maxOrder = 2)
        {
            if (string.IsNullOrWhiteSpace(text) || maxOrder < 1) return;

            text = text.Replace(".", " . ").Replace(",", " , ").Replace("?", " ? ").Replace("!", " ! ");
            var words = text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length <= maxOrder) return;
            
            // EF Core'un her işlem için yeni bir context kullanması performansı artırır.
            // Using ifadesi, işlem bitince context'in hafızadan temizlenmesini sağlar.
            using var context = await _contextFactory.CreateDbContextAsync();

            for (int i = 0; i <= words.Length - maxOrder - 1; i++)
            {
                // Değişken dereceli N-gram'ları işle (Katz's Back-off için hazırlık)
                for (int order = 1; order <= maxOrder; order++)
                {
                    var keyWords = words.Skip(i).Take(order).ToArray();
                    var key = string.Join(KeyDelimiter, keyWords);
                    var nextWord = words[i + order];

                    var ngram = await context.Ngrams
                        .FirstOrDefaultAsync(n => n.Key == key && n.NextWord == nextWord);

                    if (ngram != null)
                    {
                        // Bu N-gram daha önce görülmüş, sayacını artır
                        ngram.Count++;
                    }
                    else
                    {
                        // Bu N-gram yeni, veritabanına ekle
                        var newNgram = new Ngram
                        {
                            Key = key,
                            NextWord = nextWord,
                            Count = 1
                        };
                        await context.Ngrams.AddAsync(newNgram);
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        // Belirli bir anahtarı takip eden olası kelimeleri ve sayılarını getiren metot
        public async Task<List<Ngram>> GetPossibleNextWordsAsync(string key)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Ngrams
                .Where(n => n.Key == key)
                .ToListAsync();
        }
    }
}