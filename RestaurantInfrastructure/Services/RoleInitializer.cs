using Microsoft.AspNetCore.Identity;
using RestDomain.Models;
using System.Threading.Tasks;

namespace RestaurantInfrastructure
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Списки ваших реальних ролей
            string[] roleNames = { "Manager", "Instructor", "Worker" };

            foreach (var roleName in roleNames)
            {
                // Перевіряємо, чи існує роль, якщо ні — створюємо
                if (await roleManager.FindByNameAsync(roleName) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Створення головного аккаунта (Менеджера) для входу
            string adminEmail = "admin@mcdonalds.com";
            string password = "AdminPassword123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                User admin = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    Year = 2026,
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    // Призначаємо йому роль Manager, щоб він мав повний доступ
                    await userManager.AddToRoleAsync(admin, "Manager");
                }
            }

            // 3. Опціонально: автоматично призначити роль Менеджера вашому існуючому аккаунту
            string olgaEmail = "olga.berezhnaya20006@gmail.com";
            User olga = await userManager.FindByEmailAsync(olgaEmail);
            if (olga != null)
            {
                if (!await userManager.IsInRoleAsync(olga, "Manager"))
                {
                    await userManager.AddToRoleAsync(olga, "Manager");
                }
            }
        }
    }
}