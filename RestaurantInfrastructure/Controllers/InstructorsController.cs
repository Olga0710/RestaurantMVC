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
    [Authorize(Roles = "Manager, Instructor, Worker")]
    public class InstructorsController : Controller
    {
        private readonly DbRestaurantContext _context;

        public InstructorsController(DbRestaurantContext context)
        {
            _context = context;
        }

        // GET: Instructors
        public async Task<IActionResult> Index()
        {
            var dbRestaurantContext = _context.Instructors.Include(i => i.Employee).Include(i => i.Manager);
            return View(await dbRestaurantContext.ToListAsync());
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.Employee)
                .Include(i => i.Manager)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employers, "Id", "Id");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "EmloyeeId", "EmloyeeId");
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("EmployeeId,Specialization,ExperienceYears,ManagerId")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employers, "Id", "Id", instructor.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "EmloyeeId", "EmloyeeId", instructor.ManagerId);
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employers, "Id", "Id", instructor.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "EmloyeeId", "EmloyeeId", instructor.ManagerId);
            return View(instructor);
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,Specialization,ExperienceYears,ManagerId")] Instructor instructor)
        {
            if (id != instructor.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employers, "Id", "Id", instructor.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "EmloyeeId", "EmloyeeId", instructor.ManagerId);
            return View(instructor);
        }

        // GET: Instructors/Delete/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.Employee)
                .Include(i => i.Manager)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.EmployeeId == id);
        }
    }
}
