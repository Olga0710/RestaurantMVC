using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestDomain.Models;

namespace RestaurantInfrastructure.Controllers
{
    // Доступ мають усі авторизовані користувачі з цими ролями
    [Authorize(Roles = "Manager, Instructor, Worker")]
    public class ShiftsController : Controller
    {
        private readonly DbRestaurantContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShiftsController(DbRestaurantContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ГОЛОВНА СТОРІНКА (Index)
        public async Task<IActionResult> Index(string searchString, DateOnly? startDate, DateOnly? endDate)
        {
            var query = _context.Shifts
                .Include(s => s.Employee)
                .Include(s => s.ShiftType)
                .AsQueryable();

            // Логіка прав доступу
            if (User.IsInRole("Manager") || User.IsInRole("Instructor"))
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(s => s.Employee.LastName.Contains(searchString) || s.Employee.FirstName.Contains(searchString));
                }
            }
            else
            {
                // Worker бачить тільки свої зміни за Email
                // Переконайтеся, що в моделі Employer є поле Email!
                query = query.Where(s => s.Employee.Email == User.Identity.Name);
            }

            // Фільтрація за датами
            if (startDate.HasValue)
            {
                if (!endDate.HasValue)
                    query = query.Where(s => s.ShiftDate == startDate.Value);
                else
                    query = query.Where(s => s.ShiftDate >= startDate.Value && s.ShiftDate <= endDate.Value);
            }

            var shifts = await query.OrderByDescending(s => s.ShiftDate).ToListAsync();
            ViewBag.CurrentFilter = searchString;
            return View(shifts);
        }

        private void PopulateSelectLists(Shift? shift = null)
        {
            var emps = _context.Employers
                .Select(e => new { Id = e.Id, Name = e.LastName + " " + e.FirstName })
                .OrderBy(e => e.Name).ToList();

            ViewBag.EmployeeId = new SelectList(emps, "Id", "Name", shift?.EmployeeId);
            ViewBag.ShiftTypeId = new SelectList(_context.ShiftsTypes.OrderBy(t => t.Name), "Id", "Name", shift?.ShiftTypeId);
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("EmployeeId,ShiftTypeId,StartTime,EndTime,ShiftDate")] Shift shift)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateSelectLists(shift);
            return View(shift);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null) return NotFound();
            PopulateSelectLists(shift);
            return View(shift);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,ShiftTypeId,StartTime,EndTime,ShiftDate")] Shift shift)
        {
            if (id != shift.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(shift); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!_context.Shifts.Any(e => e.Id == shift.Id)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index));
            }
            PopulateSelectLists(shift);
            return View(shift);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var shift = await _context.Shifts
                .Include(s => s.Employee)
                .Include(s => s.ShiftType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (shift == null) return NotFound();
            return View(shift);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift != null) _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var shift = await _context.Shifts
                .Include(s => s.Employee)
                .Include(s => s.ShiftType)
                .FirstOrDefaultAsync(m => m.Id == id);

            return shift == null ? NotFound() : View(shift);
        }
    }
}