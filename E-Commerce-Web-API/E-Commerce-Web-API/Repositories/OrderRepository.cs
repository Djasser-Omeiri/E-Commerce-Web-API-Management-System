using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Interfaces;

namespace E_Commerce_Web_API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
