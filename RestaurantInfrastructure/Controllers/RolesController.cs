using RestaurantInfrastructure.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestDomain.Models; // Твій User тут
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantInfrastructure.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Відображає список усіх ролей
        public IActionResult Index() => View(_roleManager.Roles.ToList());

        // Відображає список усіх користувачів
        public IActionResult UserList() => View(_userManager.Users.ToList());

        // Метод для відкриття сторінки редагування ролей конкретного користувача
        public async Task<IActionResult> Edit(string userId)
        {
            // Отримуємо користувача з бази RestaurantIdentity
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Список ролей, які вже має цей користувач
                var userRoles = await _userManager.GetRolesAsync(user);
                // Всі ролі, що існують у системі
                var allRoles = _roleManager.Roles.ToList();

                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            // Отримуємо користувача
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Поточні ролі користувача
                var userRoles = await _userManager.GetRolesAsync(user);

                // Ролі, які користувач вибрав на формі (CheckBox)
                // Визначаємо, які ролі треба додати, а які — видалити
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("UserList");
            }

            return NotFound();
        }
    }
}