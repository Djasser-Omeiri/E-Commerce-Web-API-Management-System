namespace E_Commerce_Web_API.DTOs.OrderItem
{
    public class OrderItemDTO
    {
        public int ID { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public string ProductName { get; set; } = string.Empty;

    }
}
