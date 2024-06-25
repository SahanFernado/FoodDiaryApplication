using Entities;
using FoodDiaryApp.Models;
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
        public async Task<IActionResult> Register([FromBody] UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                var newUser = new User
                {
                    Username = user.UserName,
                    Password = hashedPassword
                };

                _context.Users.Add(newUser);
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
        public IActionResult Login([FromBody] UserViewModel user)
        {
            var loginUser = _context.Users
                .FirstOrDefault(u => u.Username == user.UserName);

            if (loginUser != null && BCrypt.Net.BCrypt.Verify(user.Password, loginUser.Password))
            {
                return Json(new { Success = true });
            }

            
            return Json(new { Success = false });
        }
    }
}
