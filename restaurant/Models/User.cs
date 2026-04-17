using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema; // Потрібно для ForeignKey

namespace RestDomain.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }

        // Додаємо ID працівника (може бути порожнім, якщо це просто адмін)
        public int? EmployerId { get; set; }

        // Навігаційна властивість, щоб легко діставати дані працівника (ПІБ, посаду)
        [ForeignKey("EmployerId")]
        public virtual Employer? Employer { get; set; }
    }
}