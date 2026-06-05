namespace E_Commerce_Web_API.Models
{
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
