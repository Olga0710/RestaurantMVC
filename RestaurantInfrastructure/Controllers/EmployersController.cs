using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestaurantInfrastructure.Services;
using RestDomain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantInfrastructure.Controllers
{
    [Authorize(Roles = "Manager, Instructor, Worker")]
    public class EmployersController : Controller
    {
        private readonly DbRestaurantContext _context;
        private readonly IDataPortServiceFactory<Employer> _factory;

        // ОДИН конструктор для всіх залежностей
        public EmployersController(DbRestaurantContext context, IDataPortServiceFactory<Employer> factory)
        {
            _context = context;
            _factory = factory;
        }

        // GET: Employers
        public async Task<IActionResult> Index(string searchString, DateOnly? startDate, DateOnly? endDate)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            var query = _context.Employers
                .Include(e => e.Shifts)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(e => e.LastName.Contains(searchString) || e.FirstName.Contains(searchString));
            }

            var employers = await query.ToListAsync();
            var stats = new Dictionary<int, (double Hours, decimal Salary)>();

            foreach (var emp in employers)
            {
                var filteredShifts = emp.Shifts.AsEnumerable();

                if (startDate.HasValue)
                    filteredShifts = filteredShifts.Where(s => s.ShiftDate >= startDate.Value);
                if (endDate.HasValue)
                    filteredShifts = filteredShifts.Where(s => s.ShiftDate <= endDate.Value);

                double totalHours = filteredShifts.Sum(s =>
                {
                    if (!s.StartTime.HasValue || !s.EndTime.HasValue) return 0;
                    TimeSpan duration = s.EndTime.Value - s.StartTime.Value;
                    if (duration.TotalHours < 0) duration = duration.Add(TimeSpan.FromHours(24));
                    return duration.TotalHours;
                });

                decimal hourlyRate = emp.SalaryPerHour ?? 0;
                decimal totalSalary = (decimal)totalHours * hourlyRate;

                stats.Add(emp.Id, (Math.Round(totalHours, 1), Math.Round(totalSalary, 2)));
            }

            ViewBag.Stats = stats;
            return View(employers);
        }

        // GET: Employers/Details/5
        public async Task<IActionResult> Details(int? id, DateOnly? startDate, DateOnly? endDate)
        {
            if (id == null) return NotFound();

            var employer = await _context.Employers
                .Include(e => e.Shifts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employer == null) return NotFound();

            var shiftsQuery = employer.Shifts.AsEnumerable();
            if (startDate.HasValue)
                shiftsQuery = shiftsQuery.Where(s => s.ShiftDate >= startDate.Value);
            if (endDate.HasValue)
                shiftsQuery = shiftsQuery.Where(s => s.ShiftDate <= endDate.Value);

            var filteredShifts = shiftsQuery.OrderByDescending(s => s.ShiftDate).ToList();

            double totalHours = filteredShifts.Sum(s => {
                if (!s.StartTime.HasValue || !s.EndTime.HasValue) return 0;
                TimeSpan duration = s.EndTime.Value - s.StartTime.Value;
                if (duration.TotalHours < 0) duration = duration.Add(TimeSpan.FromHours(24));
                return duration.TotalHours;
            });

            ViewBag.FilteredShifts = filteredShifts;
            ViewBag.TotalHours = Math.Round(totalHours, 1);
            ViewBag.TotalSalary = Math.Round((decimal)totalHours * (employer.SalaryPerHour ?? 0), 2);

            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(employer);
        }

        // GET: Employers/Create
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create(Employer employer, int? InstructorId, int? ManagerId)
        {
            if (!string.IsNullOrEmpty(employer.PhoneNumber) && await _context.Employers.AnyAsync(e => e.PhoneNumber == employer.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Цей номер телефону вже використовується!");
            }

            if (ModelState.IsValid)
            {
                _context.Add(employer);
                await _context.SaveChangesAsync();

                if (employer.Role == "Worker")
                {
                    _context.Workers.Add(new Worker { EmployeeId = employer.Id, InstructorId = InstructorId });
                }
                else if (employer.Role == "Instructor")
                {
                    _context.Instructors.Add(new Instructor { EmployeeId = employer.Id, ManagerId = ManagerId });
                }
                else if (employer.Role == "Manager")
                {
                    _context.Managers.Add(new Manager { EmployeeId = employer.Id });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropDowns(InstructorId, ManagerId);
            return View(employer);
        }

        // GET: Employers/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employer = await _context.Employers.FindAsync(id);
            if (employer == null) return NotFound();

            var worker = await _context.Workers.FirstOrDefaultAsync(w => w.EmployeeId == id);
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.EmployeeId == id);

            PopulateDropDowns(worker?.InstructorId, instructor?.ManagerId);
            return View(employer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, Employer employer, int? InstructorId, int? ManagerId)
        {
            if (id != employer.Id) return NotFound();

            if (await _context.Employers.AnyAsync(e => e.PhoneNumber == employer.PhoneNumber && e.Id != id))
            {
                ModelState.AddModelError("PhoneNumber", "Цей номер вже зайнятий іншим працівником!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employer);

                    if (employer.Role == "Worker")
                    {
                        var worker = await _context.Workers.FirstOrDefaultAsync(w => w.EmployeeId == id);
                        if (worker != null) { worker.InstructorId = InstructorId; _context.Update(worker); }
                        else { _context.Workers.Add(new Worker { EmployeeId = id, InstructorId = InstructorId }); }
                    }
                    else if (employer.Role == "Instructor")
                    {
                        var instr = await _context.Instructors.FirstOrDefaultAsync(i => i.EmployeeId == id);
                        if (instr != null) { instr.ManagerId = ManagerId; _context.Update(instr); }
                        else { _context.Instructors.Add(new Instructor { EmployeeId = id, ManagerId = ManagerId }); }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployerExists(employer.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateDropDowns(InstructorId, ManagerId);
            return View(employer);
        }

        // GET: Employers/Delete/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var employer = await _context.Employers.FirstOrDefaultAsync(m => m.Id == id);
            if (employer == null) return NotFound();
            return View(employer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employer = await _context.Employers
                .Include(e => e.Shifts)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employer != null)
            {
                if (employer.Shifts.Any()) _context.Shifts.RemoveRange(employer.Shifts);

                var workersAsInstructor = _context.Workers.Where(w => w.InstructorId == id);
                foreach (var w in workersAsInstructor) w.InstructorId = null;

                var workersAsManager = _context.Workers.Where(w => w.ManagerId == id);
                foreach (var w in workersAsManager) w.ManagerId = null;

                var workerRole = await _context.Workers.FirstOrDefaultAsync(w => w.EmployeeId == id);
                if (workerRole != null) _context.Workers.Remove(workerRole);

                var subInstr = _context.Instructors.Where(i => i.ManagerId == id);
                foreach (var i in subInstr) i.ManagerId = null;

                var instructorRole = await _context.Instructors.FirstOrDefaultAsync(i => i.EmployeeId == id);
                if (instructorRole != null) _context.Instructors.Remove(instructorRole);

                var managerRole = await _context.Managers.FirstOrDefaultAsync(m => m.EmployeeId == id);
                if (managerRole != null) _context.Managers.Remove(managerRole);

                _context.Employers.Remove(employer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // --- МЕТОДИ ДЛЯ ЕКСПОРТУ ТА ІМПОРТУ ---

        [HttpGet]
        public IActionResult Import() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken)
        {
            if (fileExcel != null && fileExcel.Length > 0)
            {
                var importService = _factory.GetImportService(fileExcel.ContentType);
                using var stream = fileExcel.OpenReadStream();
                await importService.ImportFromStreamAsync(stream, cancellationToken);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Export(CancellationToken cancellationToken)
        {
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var exportService = _factory.GetExportService(contentType);

            var memoryStream = new MemoryStream();
            await exportService.WriteToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            return File(memoryStream, contentType, $"Employers_{DateTime.Now:ddMMyyyy}.xlsx");
        }

        // --- ДОПОМІЖНІ МЕТОДИ ---

        private void PopulateDropDowns(int? selectedInstructor = null, int? selectedManager = null)
        {
            var instructors = _context.Instructors.Include(i => i.Employee)
                .Select(i => new { Id = i.EmployeeId, Name = (i.Employee.LastName ?? "") + " " + (i.Employee.FirstName ?? "") })
                .ToList();

            var managers = _context.Managers.Include(m => m.Employee)
                .Select(m => new { Id = m.EmployeeId, Name = (m.Employee.LastName ?? "") + " " + (m.Employee.FirstName ?? "") })
                .ToList();

            ViewData["InstructorId"] = new SelectList(instructors, "Id", "Name", selectedInstructor);
            ViewData["ManagerId"] = new SelectList(managers, "Id", "Name", selectedManager);
        }

        private bool EmployerExists(int id) => _context.Employers.Any(e => e.Id == id);
    }
}