// CartController.cs

using Cafe.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cafe.Controllers
{
    public class CartController : Controller
    {
        private readonly CartModel _cart;

        public CartController()
        {
            // Инициализация корзины при создании контроллера
            _cart = new CartModel();
        }

        public IActionResult Index()
        {
            // Передача корзины в представление
            return View(_cart);
        }

        public IActionResult AddToCart(int productId, string productName, int quantity, decimal price)
        {
            // Добавление товара в корзину
            _cart.Items.Add(new CartItemModel
            {
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity,
                Price = price
            });

            // Перенаправление на страницу корзины
            return RedirectToAction("Index");
        }
    }
}
