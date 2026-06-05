using E_Commerce_Web_API.DTOs;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDTO>> GetCategoriesAsync();
        Task<CategoryDTO?> GetCategoryByIdAsync(int id);
        Task<Category?> GetCategoryEntityByIdAsync(int id);
        Task<Category> CreateCategoryAsync(CreateCategoryDTO categorydto);
        Task DeleteCategoryAsync(Category category);
        Task SaveChangesAsync();

    }
}
