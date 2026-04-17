using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace RestaurantInfrastructure.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }

        // Список усіх ролей, які існують у системі (наприклад: admin, manager, employee)
        public List<IdentityRole> AllRoles { get; set; }

        // Список ролей, які вже призначені конкретному користувачу
        public IList<string> UserRoles { get; set; }

        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}