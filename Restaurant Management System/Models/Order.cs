using System;

namespace Restaurant_Management_System.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public int ClientId { get; set; }
        public int ServerId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderTime { get; set; }
    }
}