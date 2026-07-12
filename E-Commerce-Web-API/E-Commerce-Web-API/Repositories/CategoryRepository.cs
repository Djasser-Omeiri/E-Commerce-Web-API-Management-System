using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs.Category;
using E_Commerce_Web_API.Interfaces.Repositories;
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

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            return category;
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
        }

        public async Task<IQueryable<Category>> GetCategoriesAsync()
        {
            return _context.Categories
                .AsNoTracking();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task<Category?> GetCategoryEntityByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.ID == id);
        }
    }
}
