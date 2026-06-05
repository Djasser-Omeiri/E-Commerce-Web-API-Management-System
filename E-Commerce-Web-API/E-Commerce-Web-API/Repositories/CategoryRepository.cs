using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Category> CreateCategoryAsync(CreateCategoryDTO categorydto)
        {
            var category = new Category
            {
                Name = categorydto.Name
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
        {
            var categoriesDTOs = await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryDTO
                {
                    ID = c.ID,
                    Name = c.Name,
                    ProductsCount = c.Products.Count()
                }).ToListAsync();
            return categoriesDTOs;
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryDTO
                {
                    ID = c.ID,
                    Name = c.Name,
                    ProductsCount = c.Products.Count()
                }).FirstOrDefaultAsync(c => c.ID == id);
            return category;
        }

        public async Task<Category?> GetCategoryEntityByIdAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.ID == id);
            return category;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
