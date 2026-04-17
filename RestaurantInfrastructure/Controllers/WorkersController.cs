using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantInfrastructure.Controllers
{
    // Переглядати список та деталі можуть усі ролі
    [Authorize(Roles = "Manager, Instructor, Worker")]
    public class WorkersController : Controller
    {
        private readonly DbRestaurantContext _context;

        public WorkersController(DbRestaurantContext context)
        {
            _context = context;
        }

        // --- ПЕРЕГЛЯД (Доступно: Manager, Instructor, Worker) ---

        public async Task<IActionResult> Index()
        {
            var workers = _context.Workers
                .Include(w => w.Employee)
                .Include(w => w.Instructor).ThenInclude(i => i.Employee)
                .Include(w => w.Manager).ThenInclude(m => m.Employee);
            return View(await workers.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var worker = await _context.Workers
                .Include(w => w.Employee)
                .Include(w => w.Instructor)
                .Include(w => w.Manager)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            return worker == null ? NotFound() : View(worker);
        }

        // --- УПРАВЛІННЯ ПЕРСОНАЛОМ (Тільки Manager) ---

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public IActionResult Create()
        {
            var existingWorkerIds = _context.Workers.Select(w => w.EmployeeId).ToList();

            var potentialWorkers = _context.Employers
                .Where(e => !existingWorkerIds.Contains(e.Id))
                .Select(e => new {
                    Id = e.Id,
                    FullName = e.LastName + " " + e.FirstName + " (" + e.PhoneNumber + ")"
                }).ToList();

            ViewBag.EmployeeId = new SelectList(potentialWorkers, "Id", "FullName");
            PopulateDropDownLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create(Worker worker)
        {
            try
            {
                if (worker.EmployeeId > 0)
                {
                    _context.Workers.Add(worker);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Будь ласка, оберіть працівника зі списку");
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", "Помилка: " + msg);
            }

            PopulateDropDownLists(worker);
            return View(worker);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var worker = await _context.Workers.Include(w => w.Employee).FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (worker == null) return NotFound();
            PopulateDropDownLists(worker);
            return View(worker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,ManagerId,InstructorId")] Worker worker)
        {
            if (id != worker.EmployeeId) return NotFound();

            var workerInDb = await _context.Workers.FirstOrDefaultAsync(w => w.EmployeeId == id);
            if (workerInDb == null) return NotFound();

            workerInDb.ManagerId = worker.ManagerId;
            workerInDb.InstructorId = worker.InstructorId;

            // Ручне оновлення статусів (чекбоксів)
            workerInDb.IsMinor = Request.Form["IsMinor"].Contains("true");
            workerInDb.IsCertified = Request.Form["IsCertified"].Contains("true");

            try
            {
                _context.Entry(workerInDb).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Помилка бази: " + ex.Message);
                PopulateDropDownLists(workerInDb);
                return View(workerInDb);
            }
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var worker = await _context.Workers.Include(w => w.Employee).FirstOrDefaultAsync(m => m.EmployeeId == id);
            return worker == null ? NotFound() : View(worker);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worker = await _context.Workers.FindAsync(id);
            if (worker != null) _context.Workers.Remove(worker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Приватний метод для випадаючих списків
        private void PopulateDropDownLists(Worker worker = null)
        {
            var instructors = _context.Instructors.Include(i => i.Employee)
                .Select(i => new { Id = i.EmployeeId, FullName = i.Employee.LastName + " " + i.Employee.FirstName }).ToList();

            var managers = _context.Managers.Include(m => m.Employee)
                .Select(m => new { Id = m.EmployeeId, FullName = m.Employee.LastName + " " + m.Employee.FirstName }).ToList();

            ViewData["InstructorId"] = new SelectList(instructors, "Id", "FullName", worker?.InstructorId);
            ViewData["ManagerId"] = new SelectList(managers, "Id", "FullName", worker?.ManagerId);
        }
    }
}