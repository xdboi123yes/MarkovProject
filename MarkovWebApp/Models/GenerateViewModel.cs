using System.ComponentModel.DataAnnotations;

namespace MarkovWebApp.Models
{
    public class GenerateViewModel
    {
        [Required]
        public string SeedPhrase { get; set; } = string.Empty;
        public string? GeneratedText { get; set; }
    }
}