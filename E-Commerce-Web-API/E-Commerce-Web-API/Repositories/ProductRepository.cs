using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs;
using E_Commerce_Web_API.Interfaces;
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
        public async Task<Product> CreateProductAsync(CreateProductDTO productDTO)
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Price = productDTO.Price,
                Description = productDTO.Description,
                CategoryID = productDTO.CategoryID
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteProductAsync(Product product)
        {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var productDTO = await _context.Products
                .AsNoTracking()
                .Select(p => new ProductDTO
                {
                    ID = p.ID,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name ?? string.Empty
                })
                .FirstOrDefaultAsync(p => p.ID == id);
            return productDTO;
        }

        public async Task<Product?> GetProductEntityByIdAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ID == id);
            return product;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            var productsDTOs = await _context.Products
               .AsNoTracking()
               .Select(p => new ProductDTO
               {
                   ID = p.ID,
                   Name = p.Name,
                   Price = p.Price,
                   CategoryName = p.Category.Name ?? string.Empty
               }).ToListAsync();
            return productsDTOs;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
