
using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs.Stock;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly AppDbContext _context;

        public StockRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateStockAsync(CreateStockDTO StockDTO)
        {
            var Stock = new Stock
            {
                Quantity = StockDTO.Quantity,
                ProductID = StockDTO.ProductID
            };
            _context.Stocks.Add(Stock);
            await _context.SaveChangesAsync();
            return Stock;
        }

        public async Task DeleteStockAsync(Stock Stock)
        {
            _context.Stocks.Remove(Stock);
            await _context.SaveChangesAsync();
        }

        public async Task<StockDTO?> GetStockByIdAsync(int id)
        {
            return await _context.Stocks
                .AsNoTracking()
                .Select(s => new StockDTO
                {
                    ID = s.ID,
                    Quantity = s.Quantity,
                    ProductName = s.Product.Name
                })
                .FirstOrDefaultAsync(s => s.ID == id);
        }

        public async Task<Stock?> GetStockEntityByIdAsync(int id)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.ID == id);
        }

        public async Task<IEnumerable<StockDTO>> GetStocksAsync()
        {
            return await _context.Stocks
                .AsNoTracking()
                .Select(s => new StockDTO
                {
                    ID = s.ID,
                    Quantity = s.Quantity,
                    ProductName = s.Product.Name
                })
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
