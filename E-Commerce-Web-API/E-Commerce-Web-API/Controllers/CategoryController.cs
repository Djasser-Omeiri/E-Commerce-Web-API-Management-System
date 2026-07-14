using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.DTOs.Category;
using E_Commerce_Web_API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace E_Commerce_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;

        public CategoryController(ICategoryService categoryService, ILogger logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType<IEnumerable<CategoryDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesAsync()
        {
            var categoriesDTOs = await _categoryService.GetCategoriesAsync();
            if (categoriesDTOs is null)
            {
                _logger.LogWarning("GetCategoriesAsync failed - no categories found.");
                return NotFound("Categories not found");
            }

            return Ok(categoriesDTOs);
        }
        [HttpGet("{id}", Name = nameof(GetCategoryByIdAsync))]
        [AllowAnonymous]
        [ProducesResponseType<CategoryDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDTO>> GetCategoryByIdAsync(int id)
        {
            if (id < 0)
            {
                _logger.LogWarning("Invalid category ID requested: {CategoryId}", id);
                return BadRequest("Invalid category ID");
            }
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category is null)
            {
                _logger.LogWarning("Category not found. ID: {CategoryId}", id);
                return NotFound("Category not found");
            }
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType<Category>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Category>> CreateCategoryAsync(CreateCategoryDTO categorydto)
        {
            var adminID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("CreateCategoryAsync called by admin: {AdminID}. Category: {CategoryName} at {Time}", adminID, categorydto?.Name, DateTime.UtcNow);
            if (categorydto is null)
            {
                _logger.LogWarning("CreateCategoryAsync failed - invalid category data. Admin: {AdminID} at {Time}", adminID, DateTime.UtcNow);
                return BadRequest("Invalid category data");
            }
            var category = await _categoryService.CreateCategoryAsync(categorydto);
            return CreatedAtRoute(nameof(GetCategoryByIdAsync), new { id = category.ID }, category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> UpdateCategoryAsync(int id, UpdateCategoryDTO categorydto)
        {
            var adminID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("UpdateCategoryAsync called by admin: {AdminID} for category ID: {CategoryId} at {Time}", adminID, id, DateTime.UtcNow);
            if (id < 0)
            {
                _logger.LogWarning("UpdateCategoryAsync failed - invalid category ID: {CategoryId}, Admin: {AdminID} at {Time}", id, adminID, DateTime.UtcNow);
                return BadRequest("Invalid category ID");
            }

            try
            {
                var updated = await _categoryService.UpdateCategoryAsync(id, categorydto);
                if (!updated)
                {
                    _logger.LogWarning("UpdateCategoryAsync failed - category not found. ID: {CategoryId}, Admin: {AdminID} at {Time}", id, adminID, DateTime.UtcNow);
                    return NotFound("Category not found");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError("UpdateCategoryAsync concurrency error for category ID: {CategoryId}, Admin: {AdminID}. Error: {Error} at {Time}", id, adminID, ex.Message, DateTime.UtcNow);
                return Conflict("The category was modified by another process. Please reload and retry.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCategoryAsync(int id)
        {
            var AdminID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("DeleteCategoryAsync called by admin: {AdminID} for category ID: {CategoryId} at {Time}", AdminID, id, DateTime.UtcNow);
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("DeleteCategoryAsync failed - category not found. ID: {CategoryId}, Admin: {AdminID} at {Time}", id, AdminID, DateTime.UtcNow);
                return NotFound("Category not found");
            }

            return NoContent();
        }
    }
}
