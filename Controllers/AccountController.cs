using System.Security.Claims;
using Cafe.Models;
using Cafe.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        // AccountController.cs
        [HttpPost]
        public async Task<IActionResult> Login(UserModel user, int adminCode)
        {
            // Проведите аутентификацию пользователя, проверив его учетные данные и роль администратора
            if (userService.IsValidUser(user.UserName, user.Password))
            {
                if (userService.IsAdminCodeValid(adminCode))
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "Administrator"), // Добавление роли администратора
                // Другие утверждения, которые вы можете добавить
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    ViewData["UserRole"] = "Administrator"; // Установка значения для отображения на экране

                    return RedirectToAction("Index", "Home");
                }

                // Неправильный пароль админа
                // Вход без роли администратора
                ViewData["UserRole"] = "User"; // Установка значения для отображения на экране

                var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            // Другие утверждения, которые вы можете добавить для пользователя
        };

                var userIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                return RedirectToAction("Index", "Home");
            }

            // Неправильные учетные данные или отсутствие роли администратора
            ModelState.AddModelError("", "Invalid username, password, or admin code");
            return View();
        }

        // AccountController.cs
        [Authorize(Roles = "Administrator")] // Требование, чтобы пользователь имел роль администратора для доступа к этому действию
        public IActionResult ViewUsers()
        {
            var allUsers = userService.GetAllUsers();
            return View(allUsers);
        }


        [Authorize(Roles = "Administrator")]
        public IActionResult RemoveUser(string username)
        {
            string requestingUsername = User.Identity.Name; // Получаем имя пользователя, выполняющего запрос
            userService.RemoveUser(requestingUsername, username);
            return RedirectToAction("ViewUsers");
        }



    }

}
