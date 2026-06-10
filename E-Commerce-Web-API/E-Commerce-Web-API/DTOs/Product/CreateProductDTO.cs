namespace E_Commerce_Web_API.DTOs.Product
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public int InitialStock { get; set; }
    }
}
