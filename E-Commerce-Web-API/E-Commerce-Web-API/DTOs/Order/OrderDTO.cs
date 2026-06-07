using E_Commerce_Web_API.DTOs.OrderItem;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.DTOs.Order
{
    public class OrderDTO
    {
        public int ID { get; set; }
        public DateTime OrderTime { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = string.Empty;

        public ICollection<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }
}
