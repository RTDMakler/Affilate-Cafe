namespace Cafe.Controllers
{
    // KitchenController.cs
    using Cafe.Models;
    using Cafe.Models.Cafe.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;

    [Authorize]
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

            // Добавление нового заказа в список
            orders.Add(newOrder);
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
