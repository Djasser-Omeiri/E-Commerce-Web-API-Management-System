using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Interfaces.Repositories;
using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            return order;
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _context.Orders.Remove(order);
        }


        public async Task<IEnumerable<Order>> GetOrdersFilterAsync(string? userId = null)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => userId == null || o.UserId == userId)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.ID == id);
        }

        public async Task<Order?> GetOrderEntityByIdAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.ID == id);
        }
    }
}
