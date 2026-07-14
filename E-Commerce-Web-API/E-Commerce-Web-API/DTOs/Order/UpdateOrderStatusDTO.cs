using E_Commerce_Web_API.Enums;

namespace E_Commerce_Web_API.DTOs.Order
{
    public class UpdateOrderStatusDTO
    {
        public eOrderStatus NewStatus { get; set; }
    }
}
