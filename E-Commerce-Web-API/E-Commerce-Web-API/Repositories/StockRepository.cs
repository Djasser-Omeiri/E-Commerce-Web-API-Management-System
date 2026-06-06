
using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Interfaces;

namespace E_Commerce_Web_API.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly AppDbContext _context;
        
        public StockRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
