using System.ComponentModel.DataAnnotations;

namespace RestaurantInfrastructure.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введіть Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введіть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запам'ятати?")]
        public bool RememberMe { get; set; }

        // Посилання для повернення на сторінку, з якої прийшов користувач
        public string? ReturnUrl { get; set; }
    }
}