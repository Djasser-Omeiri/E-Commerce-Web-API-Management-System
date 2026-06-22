using E_Commerce_Web_API.DTOs.Category;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Category> CreateCategoryAsync(CreateCategoryDTO categorydto)
        {
            var category = new Category
            {
                Name = categorydto.Name
            };
            await _unitOfWork.Categories.CreateCategoryAsync(category);
            await _unitOfWork.CompleteAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetCategoryEntityByIdAsync(id);
            if (category == null) return false;
            await _unitOfWork.Categories.DeleteCategoryAsync(category);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetCategoriesAsync();
            return categories.Select(c => new CategoryDTO
            {
                ID = c.ID,
                Name = c.Name,
                ProductsCount = c.Products.Count()
            });
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(id);
            if (category == null) return null;

            return new CategoryDTO
            {
                ID = category.ID,
                Name = category.Name,
                ProductsCount = category.Products.Count()
            };
        }

        public async Task<Category?> GetCategoryEntityByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetCategoryEntityByIdAsync(id);
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDTO categoryDTO)
        {
            var existing = await _unitOfWork.Categories.GetCategoryEntityByIdAsync(id);
            if (existing == null) return false;

            existing.Name = categoryDTO.Name;

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
