using Entities;
using FoodDiaryApp.Models;
using FoodDiaryApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDiaryApp.Controllers
{
    public class FoodDiaryController : Controller
    {
        private readonly CalorieEstimatorService _calorieEstimator;
        private readonly AiNutritionService _aiNutritionService;
        private readonly ApplicationDbContext _context;

        public FoodDiaryController(ApplicationDbContext context, CalorieEstimatorService calorieEstimatorService, AiNutritionService aiNutritionService)
        {
            _context = context;
            _calorieEstimator = calorieEstimatorService;
            _aiNutritionService = aiNutritionService;
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
                    Id = e.Id,
                    Date = e.Date,
                    MealType = e.MealType,
                    Description = e.Description,
                    Symptoms = e.Symptoms,
                    Analysis = e.Analysis
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

                var analysisResult = await _aiNutritionService.GetCaloriesForFoodItemAsync(entry.Description);
                //var isConnected = await _calorieEstimator.TestConnection();
                //var calorieEstimate = await _calorieEstimator.EstimateCalories(entry.Description);

                var newEntry = new FoodDiaryEntry
                {
                    UserId = userId.Value,
                    Date = entry.Date,
                    MealType = entry.MealType,
                    Description = entry.Description,
                    Symptoms = entry.Symptoms,
                    Analysis = analysisResult
                };

                _context.FoodDiaryEntries.Add(newEntry);
                await _context.SaveChangesAsync();

                return Json(new { Success = true });
            }

            return Json(new { Success = false });
        }

        [HttpGet]
        public IActionResult GetSelectedEntry(int id)
        {
            var entry = _context.FoodDiaryEntries
                .Where(e => e.Id == id)
                .Select(e => new FoodDiaryEntryViewModel
                {
                    Id = e.Id,
                    Date = e.Date,
                    MealType = e.MealType,
                    Description = e.Description,
                    Symptoms = e.Symptoms,
                    Analysis = e.Analysis
                })
                .FirstOrDefault();

            return Json(entry);
        }
    }
}
