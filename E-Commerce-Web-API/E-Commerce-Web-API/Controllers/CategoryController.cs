using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.DTOs.Category;
using E_Commerce_Web_API.Interfaces.Services;

namespace E_Commerce_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType<CategoryDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesAsync()
        {
            var categoriesDTOs = await _categoryService.GetCategoriesAsync();
            if (categoriesDTOs is null)
            {
                return NotFound("Categories not found");
            }

            return Ok(categoriesDTOs);
        }
        [HttpGet("{id}", Name = nameof(GetCategoryByIdAsync))]
        [ProducesResponseType<CategoryDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDTO>> GetCategoryByIdAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest("Invalid category ID");
            }
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category is null)
            {
                return NotFound("Category not found");
            }
            return Ok(category);
        }

        [HttpPost]
        [ProducesResponseType<CategoryDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<Category>> CreateCategoryAsync(CreateCategoryDTO categorydto)
        {
            if (categorydto is null)
            {
                return BadRequest("Invalid category data");
            }
            var category = await _categoryService.CreateCategoryAsync(categorydto);
            return CreatedAtRoute(nameof(GetCategoryByIdAsync), new { id = category.ID }, category);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCategoryAsync(int id, CreateCategoryDTO categorydto)
        {
            if (id < 0)
                return BadRequest("Invalid category ID");

            if (categorydto is null)
                return BadRequest("Invalid category data");

            var existingCategory = await _categoryService.GetCategoryEntityByIdAsync(id);
            if (existingCategory is null)
                return NotFound("Category not found");

            existingCategory.Name = categorydto.Name;

            await _categoryService.UpdateCategoryAsync(existingCategory);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCategoryAsync(int id)
        {
            var category = await _categoryService.GetCategoryEntityByIdAsync(id);
            if (category is null)
            {
                return NotFound("Category not found");
            }

            await _categoryService.DeleteCategoryAsync(category);
            return NoContent();
        }
    }
}
