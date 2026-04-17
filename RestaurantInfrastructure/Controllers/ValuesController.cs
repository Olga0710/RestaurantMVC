using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestDomain.Models; // Перевірте ваш namespace

namespace RestaurantInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly DbRestaurantContext _context;

        public ChartsController(DbRestaurantContext context)
        {
            _context = context;
        }

        // Моделі для JSON відповідей
        private record CountByRoleResponseItem(string Role, int Count);
        private record HoursByDateResponseItem(string Date, double TotalHours);

        // 1. Дані для діаграми ролей
        [HttpGet("countByRole")]
        public async Task<JsonResult> GetCountByRole()
        {
            var data = await _context.Employers
                .GroupBy(e => e.Role)
                .Select(g => new CountByRoleResponseItem(g.Key ?? "Не вказано", g.Count()))
                .ToListAsync();

            return new JsonResult(data);
        }

        // 2. Дані для діаграми годин (за останні 7 днів)
        [HttpGet("hoursByDate")]
        public async Task<JsonResult> GetHoursByDate()
        {
            var shifts = await _context.Shifts.ToListAsync();

            var data = shifts
                .Where(s => s.ShiftDate.HasValue && s.StartTime.HasValue && s.EndTime.HasValue)
                .GroupBy(s => s.ShiftDate.Value)
                .Select(g => {
                    double totalHours = g.Sum(s => {
                        var duration = s.EndTime.Value - s.StartTime.Value;
                        return duration.TotalHours < 0 ? duration.TotalHours + 24 : duration.TotalHours;
                    });
                    return new HoursByDateResponseItem(g.Key.ToString("yyyy-MM-dd"), Math.Round(totalHours, 1));
                })
                .OrderBy(d => d.Date)
                .Take(7)
                .ToList();

            return new JsonResult(data);
        }

        [HttpGet("bonusesByEmployee")]
        public async Task<JsonResult> GetBonusesByEmployee()
        {
            var data = await _context.Bonuses
                .Include(b => b.Employee)
                .GroupBy(b => b.Employee.LastName)
                .Select(g => new { Name = g.Key, Total = g.Sum(b => b.Amount) })
                .ToListAsync();
            return new JsonResult(data);
        }
    }
}