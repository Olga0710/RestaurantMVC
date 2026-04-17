using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestDomain.Models;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantInfrastructure.Controllers
{
   // [Authorize(Roles = "Manager")] // Тільки Денис (Менеджер) зможе сюди зайти
    public class DashboardController : Controller
    {
        private readonly DbRestaurantContext _context;

        public DashboardController(DbRestaurantContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Збираємо базову статистику для шефа
            ViewBag.TotalEmployees = await _context.Employers.CountAsync();
            ViewBag.ManagersCount = await _context.Employers.CountAsync(e => e.Role == "Manager");
            ViewBag.WorkersCount = await _context.Employers.CountAsync(e => e.Role == "Worker");
            ViewBag.AverageSalary = await _context.Employers.AnyAsync()
                ? await _context.Employers.AverageAsync(e => e.SalaryPerHour)
                : 0;

            return View();
        }
    }
}