using System.Security.Claims;
using Cafe.Models;
using Cafe.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Cafe.Controllers
{
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

        [HttpPost]
        public async Task<IActionResult> Login(UserModel user, int adminCode)
        {
            if (userService.IsValidUser(user.UserName, user.Password))
            {
                if (userService.IsAdminCodeValid(adminCode))
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "Administrator"), 
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    ViewData["UserRole"] = "Administrator"; 

                    return RedirectToAction("Index", "Home");
                }

                ViewData["UserRole"] = "User"; 

                var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
        };

                var userIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var userPrincipal = new ClaimsPrincipal(userIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid username, password, or admin code");
            return View();
        }

        [Authorize(Roles = "Administrator")] 
        public IActionResult ViewUsers()
        {
            var allUsers = userService.GetAllUsers();
            return View(allUsers);
        }


        [Authorize(Roles = "Administrator")]
        public IActionResult RemoveUser(string username)
        {
            string requestingUsername = User.Identity.Name;
            userService.RemoveUser(requestingUsername, username);
            return RedirectToAction("ViewUsers");
        }



    }

}
