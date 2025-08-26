using System.ComponentModel.DataAnnotations;

namespace MarkovWebApp.Models
{
    public class TrainingViewModel
    {
        [Display(Name = "Training Text")]
        public string? InputText { get; set; }
    }
}