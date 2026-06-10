using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order?> GetOrderEntityByIdAsync(int id);
        Task<Order> CreateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);
    }
}
