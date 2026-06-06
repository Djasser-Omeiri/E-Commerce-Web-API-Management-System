namespace E_Commerce_Web_API.Models
{
    public class Review
    {
        public int ID { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;
    }
}
