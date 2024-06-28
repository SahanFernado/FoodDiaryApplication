
using System.ComponentModel.DataAnnotations;

namespace FoodDiaryApp.Models
{
    public class FoodDiaryEntryViewModel
    {
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string MealType { get; set; }

        [Required]
        public string Description { get; set; }

        public string Symptoms { get; set; } = string.Empty;

        public string Analysis { get; set; } = string.Empty;
    }
}
