using System.Diagnostics;
using Cafe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Статическое поле для хранения списка изображений
        private static List<string> _images;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;

            // Загрузка изображений только при первом запуске
            if (_images == null)
            {
                _images = LoadImagesFromFolder("images"); // Путь к папке с изображениями
            }
        }
        private List<string> LoadImagesFromFolder(string folderPath)
        {
            var wwwrootPath = _webHostEnvironment.WebRootPath;
            var imageFiles = Directory.GetFiles(Path.Combine(wwwrootPath, folderPath), "*.*")
                           .Where(fileName => fileName.ToLower().EndsWith(".jpg") || fileName.ToLower().EndsWith(".png"))
                           .Select(fileName => $"~/{folderPath}/{Path.GetFileName(fileName)}")
                           .ToList();


            return imageFiles;
        }

        public IActionResult Index()
        {
            

            var model = new HomeViewModel
            {
                Images = _images
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

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult RemoveImage(string imagePath)
        {
            // Проверка наличия изображения в списке
            if (_images.Contains(imagePath))
            {
                // Полный путь к изображению в файловой системе
                var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('~').TrimStart('/'));

                // Удаляем файл изображения
                System.IO.File.Delete(physicalPath);

                // Удаляем изображение из списка
                _images.Remove(imagePath);

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult AddImage(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var newImagePath = SaveImage(imageFile);
                _images.Add(newImagePath);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult EditImage(string oldImagePath, IFormFile newImageFile)
        {
            // Полный путь к старому изображению
            var oldPhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath.TrimStart('~').TrimStart('/'));

            // Удаляем старое изображение
            System.IO.File.Delete(oldPhysicalPath);

            // Сохраняем новое изображение
            var newImagePath = SaveImage(newImageFile);

            // Обновляем путь в списке изображений
            var index = _images.IndexOf(oldImagePath);
            if (index != -1)
            {
                _images[index] = newImagePath;
            }

            return RedirectToAction("Index");
        }

        private string SaveImage(IFormFile imageFile)
        {
            // Сохраняем новое изображение
            var newImagePath = $"~/images/{Path.GetFileName(imageFile.FileName)}";
            var newPhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, newImagePath.TrimStart('~').TrimStart('/'));
            using (var stream = new FileStream(newPhysicalPath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return newImagePath;
        }
    }
}
