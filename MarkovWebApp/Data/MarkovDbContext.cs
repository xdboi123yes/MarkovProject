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
        public DbSet<Ngram> Ngrams { get; set; }
    }
}