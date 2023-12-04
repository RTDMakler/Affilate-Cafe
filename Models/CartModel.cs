// CartModel.cs

using System.Collections.Generic;

namespace Cafe.Models
{
    public class CartModel
    {
        public List<CartItemModel> Items { get; set; }

        public CartModel()
        {
            Items = new List<CartItemModel>();
        }
    }

    public class CartItemModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
