using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs.Order;
using E_Commerce_Web_API.DTOs.OrderItem;
using E_Commerce_Web_API.Interfaces;
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

        public async Task<Order> CreateOrderAsync(CreateOrderDTO orderDTO)
        {
            var Order = new Order
            {
                ShippingAddress = orderDTO.ShippingAddress
            };
            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();
            return Order;
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<OrderDTO>> GetOrdersAsync()
        {
            var ordersDTOs = await _context.Orders
            .AsNoTracking().Select(o => new
            {
                o.ID,
                o.OrderTime,
                o.TotalPrice,
                o.ShippingAddress,
                o.Status,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductName = oi.Product.Name
                }).ToList()
            })
        .ToListAsync();

            return ordersDTOs.Select(o => new OrderDTO
            {
                ID = o.ID,
                OrderTime = o.OrderTime,
                TotalPrice = o.TotalPrice,
                ShippingAddress = o.ShippingAddress,
                Status = o.Status.ToString(),
                OrderItems = o.OrderItems
            });
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(int id)
        {
            var orderDTO = await _context.Orders
                .AsNoTracking().Select(o => new
                {
                    o.ID,
                    o.OrderTime,
                    o.TotalPrice,
                    o.ShippingAddress,
                    o.Status,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                    {
                        ProductName = oi.Product.Name
                    }).ToList()
                }).FirstOrDefaultAsync(o => o.ID == id);

            return orderDTO is null ? null : new OrderDTO
            {
                ID = orderDTO.ID,
                OrderTime = orderDTO.OrderTime,
                TotalPrice = orderDTO.TotalPrice,
                ShippingAddress = orderDTO.ShippingAddress,
                Status = orderDTO.Status.ToString(),
                OrderItems = orderDTO.OrderItems
            };
        }

        public async Task<Order?> GetOrderEntityByIdAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.ID == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
