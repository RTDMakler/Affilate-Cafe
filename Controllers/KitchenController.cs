namespace Cafe.Controllers
{
    // KitchenController.cs
    using Cafe.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;

    [Authorize]
    public class KitchenController : Controller
    {
        private static List<OrderModel> _orders = new List<OrderModel>();


        private readonly IWebHostEnvironment _webHostEnvironment;

        private static List<string> _images;
        public KitchenController( IWebHostEnvironment webHostEnvironment)
        {

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


        public IActionResult Kitchen()
        {
            FullOrderModel fullOrder = new FullOrderModel();
            fullOrder.KitchenViewModel = new KitchenViewModel();
            fullOrder.KitchenViewModel.Orders = _orders;
            fullOrder.homeViewModel = new HomeViewModel();
            fullOrder.homeViewModel.Images = _images;
            return View(fullOrder);
        }

        [HttpPost]
        public IActionResult AddOrder(OrderModel newOrder, string[] selectedWords)
        {
            // Генерация номера заказа
            newOrder.OrderNumber = GenerateOrderNumber();

            if (User.Identity.IsAuthenticated)
            {
                // Если пользователь аутентифицирован, используйте его имя
                newOrder.CustomerName = User.Identity.Name;
            }
            else
            {
                // Иначе, используйте значение из формы
                newOrder.CustomerName = Request.Form["CustomerName"];
            }

            // Генерация случайного времени, если не задано в форме
            if (newOrder.ReadyTime == DateTime.MinValue)
            {
                newOrder.ReadyTime = GenerateRandomTime();
            }
            newOrder.Goods = selectedWords;
            // Добавление нового заказа в список
            _orders.Add(newOrder);
            return RedirectToAction("Kitchen", "Kitchen");
        }

        private DateTime GenerateRandomTime()
        {
            // Генерация случайного времени в пределах ближайших 7 дней
            Random random = new Random();
            int daysToAdd = random.Next(1, 7);
            return DateTime.Now.AddDays(daysToAdd);
        }

        private int GenerateOrderNumber()
        {
            // Генерация уникального номера заказа
            Random random = new Random();
            return random.Next(1000, 9999);
        }
    }


}
