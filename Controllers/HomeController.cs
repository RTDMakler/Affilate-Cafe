using System.Diagnostics;
using Cafe.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Создаем список для передачи ссылок в представление
            var links = new List<string>
            {
                Url.Action("Page1"),
                Url.Action("Page2"),
                Url.Action("Page3"),
                Url.Action("Page4")
            };

            // Создаем список для передачи изображений в представление
            var images = new List<string>();
            for (int i = 1; i <= 10; i++)
            {
                images.Add($"~/images/image{i}.jpg");
            }

            // Создаем модель, содержащую данные
            var model = new HomeViewModel
            {
                Links = links,
                Images = images
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Registration()
        {
            return RedirectToAction("Register", "Account");
        }

        public IActionResult Kitchen()
        {
            // Логика для получения данных, если необходимо
            return RedirectToAction("Kitchen", "Kitchen");
        }
    }
}
