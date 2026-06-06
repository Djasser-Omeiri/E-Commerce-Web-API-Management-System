using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Interfaces;

namespace E_Commerce_Web_API.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
