using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs.Product;
using E_Commerce_Web_API.Interfaces.Repositories;
using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Product> CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            return product;
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Stock)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task<Product?> GetProductEntityByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Stock).FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task<IQueryable<Product>> GetProductsAsync()
        {
            return _context.Products
               .AsNoTracking();
        }
    }
}
