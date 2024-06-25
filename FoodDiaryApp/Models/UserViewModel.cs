using System.ComponentModel.DataAnnotations;

namespace FoodDiaryApp.Models
{
    public class UserViewModel
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
