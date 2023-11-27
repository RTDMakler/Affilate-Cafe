namespace Cafe.Controllers
{
    // KitchenController.cs
    using Cafe.Models;
    using Cafe.Models.Cafe.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;

    [Authorize(Roles = "Administrator")]
    public class KitchenController : Controller
    {
        private static List<OrderModel> orders = new List<OrderModel>();

        public IActionResult Kitchen()
        {
            // Отображение всех существующих заказов
            var viewModel = new KitchenViewModel
            {
                Orders = orders
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddOrder(OrderModel newOrder)
        {
            // Генерация номера заказа
            newOrder.OrderNumber = GenerateOrderNumber();

            // Добавление нового заказа в список
            orders.Add(newOrder);

            // Перенаправление на страницу существующих заказов
            return RedirectToAction("Kitchen", "Kitchen");
        }

        private int GenerateOrderNumber()
        {
            // Генерация уникального номера заказа
            Random random = new Random();
            return random.Next(1000, 9999);
        }
    }


}
