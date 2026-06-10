using E_Commerce_Web_API.DTOs.Category;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetCategoriesAsync();
        Task<CategoryDTO?> GetCategoryByIdAsync(int id);
        Task<Category?> GetCategoryEntityByIdAsync(int id);
        Task<Category> CreateCategoryAsync(CreateCategoryDTO categorydto);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
    }
}
