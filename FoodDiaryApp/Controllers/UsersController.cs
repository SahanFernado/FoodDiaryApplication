using Entities;
using Microsoft.AspNetCore.Mvc;

namespace FoodDiaryApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var loginUser = _context.Users
                .FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (loginUser != null)
            {
                // You might want to add authentication logic here
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(user);
        }
    }
}
