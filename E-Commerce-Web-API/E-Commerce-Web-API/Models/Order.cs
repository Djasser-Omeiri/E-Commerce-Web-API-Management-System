using E_Commerce_Web_API.Enums;

namespace E_Commerce_Web_API.Models
{
    public class Order
    {
        public int ID { get; set; }
        public DateTime OrderTime { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;
        public eOrderStatus Status { get; set; } = eOrderStatus.Pending;
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
