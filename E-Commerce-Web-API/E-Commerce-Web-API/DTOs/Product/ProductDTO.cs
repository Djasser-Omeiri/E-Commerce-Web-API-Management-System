namespace E_Commerce_Web_API.DTOs.Product
{
    public class ProductDTO
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }
}
