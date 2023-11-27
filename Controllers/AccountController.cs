using Cafe.Models;
using Cafe.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cafe.Controllers
{
    // Controllers/AccountController.cs
    public class AccountController : Controller
    {
        private readonly UserService userService;

        public AccountController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {

            Console.WriteLine("GET Register called"); 
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserModel newUser)
        {
            if (ModelState.IsValid)
            {
                // Проверка, что пользователь с таким именем не существует
                if (userService.GetUserByUsername(newUser.UserName) == null)
                {
                    userService.AddUser(newUser);
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("UserName", "User with this username already exists.");
                }
            }
            return View(newUser);
        }
    }

}
