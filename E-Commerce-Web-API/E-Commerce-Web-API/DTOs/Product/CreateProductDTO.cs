namespace E_Commerce_Web_API.DTOs.Product
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int CategoryID { get; set; }
    }
}
