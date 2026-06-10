using E_Commerce_Web_API.DTOs.OrderItem;

namespace E_Commerce_Web_API.DTOs.Order
{
    public class CreateOrderDTO
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public List<CreateOrderItemDTO> Items { get; set; } = new List<CreateOrderItemDTO>();
    }
}
