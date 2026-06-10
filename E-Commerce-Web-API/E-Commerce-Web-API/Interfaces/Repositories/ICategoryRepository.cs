using E_Commerce_Web_API.DTOs.Category;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category?> GetCategoryEntityByIdAsync(int id);
        Task<Category> CreateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);

    }
}
