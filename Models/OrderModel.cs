    namespace Cafe.Models
    {
        public class OrderModel
        {
            public int OrderNumber { get; set; }
            public string CustomerName { get; set; }
            public DateTime ReadyTime { get; set; }
            public string[] Goods { get; set; }
        }
    }


