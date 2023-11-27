using System.Security.Claims;
using Cafe.Models;
using Cafe.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
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


        //Authentification
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel user)
        {
            // Проведите аутентификацию пользователя, проверив его учетные данные
            if (userService.IsValidUser(user.UserName, user.Password))
            {
                // Успешная аутентификация
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            // Другие утверждения, которые вы можете добавить
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            // Неправильные учетные данные
            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


    }

}
