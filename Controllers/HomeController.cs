using System.Diagnostics;
using Cafe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private static List<string> _images;
        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;

            if (_images == null)
            {
                _images = LoadImagesFromFolder("images"); 
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
            return RedirectToAction("Kitchen", "Kitchen");
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult RemoveImage(string imagePath)
        {
            if (_images.Contains(imagePath))
            {
                var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('~').TrimStart('/'));

                System.IO.File.Delete(physicalPath);

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
            var oldPhysicalPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath.TrimStart('~').TrimStart('/'));

            System.IO.File.Delete(oldPhysicalPath);

            var newImagePath = SaveImage(newImageFile);

            var index = _images.IndexOf(oldImagePath);
            if (index != -1)
            {
                _images[index] = newImagePath;
            }

            return RedirectToAction("Index");
        }

        private string SaveImage(IFormFile imageFile)
        {
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
