// MarkovWebApp/Data/Models/Ngram.cs

using System.ComponentModel.DataAnnotations;

namespace MarkovWebApp.Data.Models
{
    public class Ngram
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; } = null!;

        [Required]
        public string NextWord { get; set; } = null!;

        public int Count { get; set; }
    }
}