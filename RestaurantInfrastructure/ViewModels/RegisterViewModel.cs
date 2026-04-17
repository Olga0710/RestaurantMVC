using System.ComponentModel.DataAnnotations;

namespace RestaurantInfrastructure.ViewModels
{
    public class RegisterViewModel
    {
        public int? EmployerId { get; set; }
        public string? Role { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Рік народження")]
        public int Year { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Паролі не збігаються")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        public string PasswordConfirm { get; set; }
    }
}