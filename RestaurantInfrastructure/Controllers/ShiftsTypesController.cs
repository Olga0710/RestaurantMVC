using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestDomain.Models;

namespace RestaurantInfrastructure.Controllers
{
    [Authorize(Roles = "Manager, Instructor, Worker")]
    public class ShiftsTypesController : Controller
    {
        private readonly DbRestaurantContext _context;

        public ShiftsTypesController(DbRestaurantContext context)
        {
            _context = context;
        }

        // Список кодів
        public async Task<IActionResult> Index()
        {
            return View(await _context.ShiftsTypes.OrderBy(x => x.Name).ToListAsync());
        }

        // Деталі
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var shiftsType = await _context.ShiftsTypes.FirstOrDefaultAsync(m => m.Id == id);
            if (shiftsType == null) return NotFound();
            return View(shiftsType);
        }

        // Створення (GET)
        [Authorize(Roles = "Manager")]
        public IActionResult Create() => View();

        // Створення (POST)
        // Створення (POST) - ОСТАТОЧНА ВЕРСІЯ
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create(string Name, string Description)
        {
            // 1. Валідація назви
            if (string.IsNullOrWhiteSpace(Name))
            {
                ModelState.AddModelError("Name", "Код позиції (абревіатура) є обов'язковим");
                return View();
            }

            try
            {
                // 2. Прямий SQL запит. Ми НЕ передаємо ID, тому база 
                // використає свій лічильник (Sequence), який ми вже налаштували на 20.
                // Це найнадійніший спосіб обійти помилку "Duplicate Key".
                string sql = "INSERT INTO \"ShiftsTypes\" (\"Name\", \"Description\") VALUES ({0}, {1})";
                await _context.Database.ExecuteSqlRawAsync(sql, Name, Description);

                // 3. Якщо все добре - повертаємось до списку
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Якщо база все одно видасть помилку (наприклад, дублікат Name), ми її побачимо
                var message = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", "Помилка збереження: " + message);
                return View();
            }
        }


        // Редагування (GET)
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var shiftsType = await _context.ShiftsTypes.FindAsync(id);
            if (shiftsType == null) return NotFound();
            return View(shiftsType);
        }

        // Редагування (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] ShiftsType shiftsType)
        {
            if (id != shiftsType.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shiftsType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftsTypeExists(shiftsType.Id)) return NotFound();
                    else throw;
                }
            }
            return View(shiftsType);
        }

        // Видалення (GET - сторінка підтвердження)
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var shiftsType = await _context.ShiftsTypes.FirstOrDefaultAsync(m => m.Id == id);
            if (shiftsType == null) return NotFound();
            return View(shiftsType);
        }

        // Видалення (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shiftsType = await _context.ShiftsTypes.FindAsync(id);
            if (shiftsType != null)
            {
                _context.ShiftsTypes.Remove(shiftsType);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ShiftsTypeExists(int id) => _context.ShiftsTypes.Any(e => e.Id == id);
    }
}