namespace E_Commerce_Web_API.Models
{
    public class OrderItem
    {
        public int ID { get; set; }
        public int Quantity { get; set; }

        public decimal PriceAtPurchase { get; set; }

        public int OrderID { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;
    }
}
