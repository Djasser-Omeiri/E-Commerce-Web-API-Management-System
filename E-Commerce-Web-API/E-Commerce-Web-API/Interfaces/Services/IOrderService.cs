using E_Commerce_Web_API.DTOs.Order;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetOrdersAsync();
        Task<OrderDTO?> GetOrderByIdAsync(int id);
        Task<Order?> GetOrderEntityByIdAsync(int id);
        Task<Order> CreateOrderAsync(CreateOrderDTO orderDTO);
        Task DeleteOrderAsync(Order order);
    }
}
