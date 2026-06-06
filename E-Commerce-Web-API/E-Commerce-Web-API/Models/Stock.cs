namespace E_Commerce_Web_API.Models
{
    public class Stock
    {
        public int ID { get; set; }
        public int Quantity { get; set; }
        
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;
    }
}
