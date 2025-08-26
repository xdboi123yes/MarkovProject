// MarkovWebApp/Data/MarkovDbContext.cs

using Microsoft.EntityFrameworkCore;
using MarkovWebApp.Data.Models;

namespace MarkovWebApp.Data
{
    public class MarkovDbContext : DbContext
    {
        public MarkovDbContext(DbContextOptions<MarkovDbContext> options) : base(options)
        {
        }

        // Bu satır, Entity Framework'e 'Benim Ngram adında bir modelim var ve bu veritabanında Ngrams
        // adında bir tabloya karşılık gelecek' der.
        public DbSet<Ngram> Ngrams { get; set; }
    }
}