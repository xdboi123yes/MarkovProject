// MarkovWebApp/Data/Models/Ngram.cs

using System.ComponentModel.DataAnnotations;

namespace MarkovWebApp.Data.Models
{
    public class Ngram
    {
        [Key] // Bu alanın birincil anahtar (Primary Key) olduğunu belirtir
        public int Id { get; set; }

        [Required] // Bu alanın boş olamayacağını belirtir
        public string Key { get; set; } = null!;

        [Required]
        public string NextWord { get; set; } = null!;

        public int Count { get; set; }
    }
}