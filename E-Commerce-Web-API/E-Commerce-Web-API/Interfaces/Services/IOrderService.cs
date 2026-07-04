using E_Commerce_Web_API.DTOs.Order;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetOrdersFilterAsync(string? userId = null);
        Task<OrderDTO?> GetOrderByIdAsync(int id);
        Task<Order?> GetOrderEntityByIdAsync(int id);
        Task<Order> CreateOrderAsync(CreateOrderDTO orderDTO, string userId);
        Task DeleteOrderAsync(Order order);
        Task<OrderDTO?> UpdateOrderAddressAsync(int id, UpdateOrderAddressDTO dto);
        Task<OrderDTO?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDTO dto);
    }
}
