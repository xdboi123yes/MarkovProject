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

        public async Task TrainAsync(string text, int maxOrder = 2)
        {
            if (string.IsNullOrWhiteSpace(text) || maxOrder < 1) return;

            text = text.Replace(".", " . ").Replace(",", " , ").Replace("?", " ? ").Replace("!", " ! ");
            var words = text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length <= maxOrder) return;
            
            using var context = await _contextFactory.CreateDbContextAsync();

            for (int i = 0; i <= words.Length - maxOrder - 1; i++)
            {
                for (int order = 1; order <= maxOrder; order++)
                {
                    var keyWords = words.Skip(i).Take(order).ToArray();
                    var key = string.Join(KeyDelimiter, keyWords);
                    var nextWord = words[i + order];

                    var ngram = await context.Ngrams
                        .FirstOrDefaultAsync(n => n.Key == key && n.NextWord == nextWord);

                    if (ngram != null)
                    {
                        ngram.Count++;
                    }
                    else
                    {
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

        public async Task<List<Ngram>> GetPossibleNextWordsAsync(string key)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Ngrams
                .Where(n => n.Key == key)
                .ToListAsync();
        }
    }
}