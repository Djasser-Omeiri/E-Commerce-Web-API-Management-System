namespace E_Commerce_Web_API.DTOs.Stock
{
    public class StockDTO
    {
        public int ID { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }
}
