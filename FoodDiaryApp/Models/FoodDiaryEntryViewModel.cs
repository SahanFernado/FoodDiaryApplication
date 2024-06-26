
using System.ComponentModel.DataAnnotations;

namespace FoodDiaryApp.Models
{
    public class FoodDiaryEntryViewModel
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string MealType { get; set; }

        [Required]
        public string Description { get; set; }

        public string Symptoms { get; set; } = string.Empty;
    }
}
