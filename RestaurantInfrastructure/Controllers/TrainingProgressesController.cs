using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantInfrastructure.Models;
using RestDomain.Models;

namespace RestaurantInfrastructure.Controllers
{
    [Route("TrainingProgress")]
    [Authorize(Roles = "Manager, Instructor, Worker")]
    public class TrainingProgressesController : Controller
    {
        private readonly DbRestaurantContext _context;

        public TrainingProgressesController(DbRestaurantContext context)
        {
            _context = context;
        }

        // GET: TrainingProgress?instructorName=Котелевець
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string instructorName)
        {
            var query = _context.TrainingProgresses
                .Include(t => t.InstuctorDNavigation).ThenInclude(i => i.Employee)
                .Include(t => t.Worker).ThenInclude(w => w.Employee)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(instructorName))
            {
                query = query.Where(t => t.InstuctorDNavigation.Employee.LastName == instructorName);
                ViewBag.CurrentInstructor = instructorName;
            }

            return View(await query.ToListAsync());
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var progress = await _context.TrainingProgresses
                .Include(t => t.InstuctorDNavigation).ThenInclude(i => i.Employee)
                .Include(t => t.Worker).ThenInclude(w => w.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (progress == null) return NotFound();
            return View(progress);
        }

        [Authorize(Roles = "Manager, Instructor")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Instructor")]
        public async Task<IActionResult> Create([Bind("InstuctorD,WorkerId,KnowledgeLevel,ReviewDate,Station")] TrainingProgress trainingProgress)
        {
            if (trainingProgress.ReviewDate == null)
                trainingProgress.ReviewDate = DateOnly.FromDateTime(DateTime.Now);

            // 1. ПЕРЕВІРКА: чи вже є оцінка по цій СТАНЦІЇ для цього РОБІТНИКА у цьому МІСЯЦІ
            var alreadyExists = await _context.TrainingProgresses.AnyAsync(t =>
                t.WorkerId == trainingProgress.WorkerId &&
                t.Station == trainingProgress.Station &&
                t.ReviewDate.Value.Month == trainingProgress.ReviewDate.Value.Month &&
                t.ReviewDate.Value.Year == trainingProgress.ReviewDate.Value.Year);

            if (alreadyExists)
            {
                ModelState.AddModelError("", $"Цей працівник вже отримав оцінку за станцію '{trainingProgress.Station}' у цьому місяці.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(trainingProgress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropDownLists(trainingProgress.InstuctorD, trainingProgress.WorkerId);
            return View(trainingProgress);
        }

        [Authorize(Roles = "Manager, Instructor")]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var progress = await _context.TrainingProgresses.FindAsync(id);
            if (progress == null) return NotFound();

            PopulateDropDownLists(progress.InstuctorD, progress.WorkerId);
            return View(progress);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Instructor")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InstuctorD,WorkerId,KnowledgeLevel,ReviewDate,Station")] TrainingProgress trainingProgress)
        {
            if (id != trainingProgress.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(trainingProgress);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(trainingProgress.InstuctorD, trainingProgress.WorkerId);
            return View(trainingProgress);
        }

        [Authorize(Roles = "Manager, Instructor")]
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var progress = await _context.TrainingProgresses
                .Include(t => t.InstuctorDNavigation).ThenInclude(i => i.Employee)
                .Include(t => t.Worker).ThenInclude(w => w.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (progress == null) return NotFound();
            return View(progress);
        }

        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Instructor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var progress = await _context.TrainingProgresses.FindAsync(id);
            if (progress != null)
            {
                _context.TrainingProgresses.Remove(progress);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        // Додай цей метод в TrainingProgressesController

        [Authorize(Roles = "Manager")]
        [HttpGet("GrantBonus/{id}")]
        public async Task<IActionResult> GrantBonus(int id)
        {
            var progress = await _context.TrainingProgresses
                .Include(t => t.Worker).ThenInclude(w => w.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (progress == null) return NotFound();

            // Логіка розрахунку суми (приклад для ресторану)
            decimal bonusAmount = 0;
            int score = progress.KnowledgeLevel ?? 0;

            if (score >= 95) bonusAmount = 1000;      // Експерт (95-100%)
            else if (score >= 90) bonusAmount = 500;   // Високий рівень (90-94%)
            else if (score >= 85) bonusAmount = 250;   // Базовий рівень (85-89%)

            string stationName = string.IsNullOrEmpty(progress.Station) ? "станцію" : progress.Station;
            string bonusReason = $"Премія за КЛС: {stationName} ({score}%).";

            // Передаємо суму (amount) разом із причиною та ID працівника
            return RedirectToAction("Create", "Bonuses", new
            {
                workerId = progress.WorkerId,
                reason = bonusReason,
                amount = bonusAmount // Додаємо цей параметр
            });
        }
        private void PopulateDropDownLists(object selectedInstructor = null, object selectedWorker = null, object selectedStation = null)
        {
            var instructors = _context.Instructors.Include(i => i.Employee)
                .Select(i => new { Id = i.EmployeeId, Name = i.Employee.LastName + " " + i.Employee.FirstName }).ToList();

            var workers = _context.Workers.Include(w => w.Employee)
                .Select(w => new { Id = w.EmployeeId, Name = w.Employee.LastName + " " + w.Employee.FirstName }).ToList();

            // Отримуємо ВСІ станції з твого довідника ShiftsTypes
            var stations = _context.ShiftsTypes
                .OrderBy(s => s.Name)
                .Select(s => new { Value = s.Name, Text = s.Name })
                .ToList();

            ViewData["InstuctorD"] = new SelectList(instructors, "Id", "Name", selectedInstructor);
            ViewData["WorkerId"] = new SelectList(workers, "Id", "Name", selectedWorker);

            // Передаємо список станцій у View
            ViewData["StationList"] = new SelectList(stations, "Value", "Text", selectedStation);
        }
    }
}