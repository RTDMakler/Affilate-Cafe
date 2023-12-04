namespace Cafe.Controllers
{
    using Cafe.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;

    [Authorize]
    public class KitchenController : Controller
    {
        private static List<OrderModel> _orders = new List<OrderModel>();
        static int _ordersCount = 0;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private static List<string> _images;
        public KitchenController( IWebHostEnvironment webHostEnvironment)
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
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteOrder(int orderNumber)
        {
            var orderToDelete = _orders.Find(o => o.OrderNumber == orderNumber);
            if (orderToDelete != null)
            {
                _orders.Remove(orderToDelete);
            }
            return RedirectToAction("Kitchen", "Kitchen");
        }

        [HttpPost]
        public IActionResult AddOrder(OrderModel newOrder, string[] selectedWords)
        {
            newOrder.OrderNumber = GenerateOrderNumber();

            if (User.Identity!.IsAuthenticated)
            {
                newOrder.CustomerName = User.Identity.Name!;
            }
            else
            {
                newOrder.CustomerName = Request.Form["CustomerName"]!;
            }

            if (newOrder.ReadyTime == DateTime.MinValue)
            {
                newOrder.ReadyTime = GenerateRandomTime();
            }
            newOrder.Goods = selectedWords;
            _orders.Add(newOrder);
            return RedirectToAction("Kitchen", "Kitchen");
        }

        private DateTime GenerateRandomTime()
        {
            Random random = new Random();
            int daysToAdd = random.Next(1, 7);
            return DateTime.Now.AddDays(daysToAdd);
        }

        private int GenerateOrderNumber()
        {
            _ordersCount++;
            return _ordersCount;
        }
    }


}
