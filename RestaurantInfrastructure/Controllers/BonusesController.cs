using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantInfrastructure.Models;
using RestDomain.Models;

namespace RestaurantInfrastructure.Controllers
{
    [Authorize(Roles = "Manager, Instructor")]
    public class BonusesController : Controller
    {
        private readonly DbRestaurantContext _context;

        public BonusesController(DbRestaurantContext context)
        {
            _context = context;
        }

        // GET: Bonuses
        public async Task<IActionResult> Index()
        {
            var query = _context.Bonuses
                .Include(b => b.Employee)
                .Include(b => b.Manager).ThenInclude(m => m.Employee)
                .AsNoTracking();

            return View(await query.ToListAsync());
        }

        // ДОДАНО: Метод Details (виправляє помилку 404)
        // GET: Bonuses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bonuse = await _context.Bonuses
                .Include(b => b.Employee)
                .Include(b => b.Manager).ThenInclude(m => m.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bonuse == null) return NotFound();

            return View(bonuse);
        }

        // GET: Bonuses/Create
        // GET: Bonuses/Create
        public IActionResult Create(int? workerId, string reason, decimal? amount)
        {
            PopulateDropDownLists(workerId);

            var model = new Bonuse
            {
                DataGranted = DateTimeOffset.Now,
                Reason = reason ?? "За результатами закриття станції",
                Amount = amount ?? 0 // Якщо сума прийшла з навчання — вона підставиться автоматично
            };

            return View(model);
        }

        // POST: Bonuses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,Amount,Reason,DataGranted,ManagerId")] Bonuse bonuse)
        {
            var currentUserName = User.Identity?.Name;
            if (!string.IsNullOrEmpty(currentUserName))
            {
                var manager = await _context.Managers
                    .Include(m => m.Employee)
                    .FirstOrDefaultAsync(m => m.Employee.LastName == currentUserName);
                if (bonuse.DataGranted == DateTimeOffset.MinValue || bonuse.DataGranted == default)
                {
                    bonuse.DataGranted = DateTimeOffset.Now;
                }

                if (manager != null)
                {
                    bonuse.ManagerId = manager.EmployeeId;
                }
            }

            // Очищення валідації для навігаційних властивостей
            ModelState.Remove("Manager");
            ModelState.Remove("Employee");

            if (ModelState.IsValid)
            {
                if (bonuse.DataGranted == null || bonuse.DataGranted == default)
                    bonuse.DataGranted = DateTimeOffset.Now;

                _context.Add(bonuse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropDownLists(bonuse.EmployeeId, bonuse.ManagerId);
            return View(bonuse);
        }

        // GET: Bonuses/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bonuse = await _context.Bonuses.FindAsync(id);
            if (bonuse == null) return NotFound();

            PopulateDropDownLists(bonuse.EmployeeId, bonuse.ManagerId);
            return View(bonuse);
        }

        // POST: Bonuses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ManagerId,EmployeeId,Amount,Reason,DataGranted")] Bonuse bonuse)
        {
            if (id != bonuse.Id) return NotFound();

            // Очищення валідації
            ModelState.Remove("Manager");
            ModelState.Remove("Employee");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bonuse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bonuses.Any(e => e.Id == bonuse.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(bonuse.EmployeeId, bonuse.ManagerId);
            return View(bonuse);
        }

        // GET: Bonuses/Delete/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bonuse = await _context.Bonuses
                .Include(b => b.Employee)
                .Include(b => b.Manager).ThenInclude(m => m.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bonuse == null) return NotFound();

            return View(bonuse);
        }

        // POST: Bonuses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bonuse = await _context.Bonuses.FindAsync(id);
            if (bonuse != null)
            {
                _context.Bonuses.Remove(bonuse);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropDownLists(object selectedEmployee = null, object selectedManager = null)
        {
            var employees = _context.Employers
                .Select(e => new { Id = e.Id, Name = e.LastName + " " + e.FirstName })
                .OrderBy(e => e.Name)
                .ToList();

            var managers = _context.Managers
                .Include(m => m.Employee)
                .Select(m => new { Id = m.EmployeeId, Name = m.Employee.LastName + " " + m.Employee.FirstName })
                .OrderBy(m => m.Name)
                .ToList();

            ViewData["EmployeeId"] = new SelectList(employees, "Id", "Name", selectedEmployee);
            ViewData["ManagerId"] = new SelectList(managers, "Id", "Name", selectedManager);
        }
    }
}