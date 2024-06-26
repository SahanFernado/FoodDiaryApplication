using Entities;
using FoodDiaryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDiaryApp.Controllers
{
    public class FoodDiaryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoodDiaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Users");
            }

            var entries = _context.FoodDiaryEntries
                .Where(e => e.UserId == userId)
                .Select(e => new FoodDiaryEntryViewModel
                {
                    Date = e.Date,
                    MealType = e.MealType,
                    Description = e.Description,
                    Symptoms = e.Symptoms
                })
                .ToList();

            return View(entries);
        }

        [HttpGet]
        public IActionResult CreateEntry()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEntry([FromBody] FoodDiaryEntryViewModel entry)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Users");
                }

                var newEntry = new FoodDiaryEntry
                {
                    UserId = userId.Value,
                    Date = entry.Date,
                    MealType = entry.MealType,
                    Description = entry.Description,
                    Symptoms = entry.Symptoms
                };

                _context.FoodDiaryEntries.Add(newEntry);
                await _context.SaveChangesAsync();

                return Json(new { Success = true });
            }

            return Json(new { Success = false });
        }
    }
}
