using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs.OrderItem;
using E_Commerce_Web_API.Interfaces.Repositories;
using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItem> CreateOrderItemAsync(CreateOrderItemDTO orderItemDTO)
        {
            var OrderItem = new OrderItem
            {
                Quantity = orderItemDTO.Quantity,
                ProductID = orderItemDTO.ProductID
            };
            _context.OrderItems.Add(OrderItem);
            return OrderItem;
        }

        public async Task DeleteOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Remove(orderItem);
        }

        public async Task<OrderItemDTO?> GetOrderItemByIdAsync(int id)
        {
            return await _context.OrderItems
                .AsNoTracking()
                .Select(oi => new OrderItemDTO
                {
                    ID = oi.ID,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase,
                    OrderID = oi.OrderID,
                    ProductName = oi.Product.Name
                })
                .FirstOrDefaultAsync(oi => oi.ID == id);
        }

        public async Task<OrderItem?> GetOrderItemEntityByIdAsync(int id)
        {
            return await _context.OrderItems.FirstOrDefaultAsync(oi => oi.ID == id);
        }

        public async Task<IEnumerable<OrderItemDTO>> GetOrderItemsAsync()
        {
            return await _context.OrderItems
                .AsNoTracking()
                .Select(oi => new OrderItemDTO
                {
                    ID = oi.ID,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase,
                    OrderID = oi.OrderID,
                    ProductName = oi.Product.Name
                }).ToListAsync();
        }
    }
}