using E_Commerce_Web_API.DTOs.OrderItem;
using E_Commerce_Web_API.DTOs.Product;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<IEnumerable<OrderItemDTO>> GetOrderItemsAsync();
        Task<OrderItemDTO?> GetOrderItemByIdAsync(int id);
        Task<OrderItem?> GetOrderItemEntityByIdAsync(int id);
        Task<OrderItem> CreateOrderItemAsync(CreateOrderItemDTO orderItemDTO);
        Task DeleteOrderItemAsync(OrderItem orderItem);
        Task SaveChangesAsync();
    }
}
