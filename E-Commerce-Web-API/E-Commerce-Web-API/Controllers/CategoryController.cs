using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            if (categories is null)
            {
                return NotFound();
            }
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryByIdAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
            {
                return NotFound("Category not found");
            }
            return Ok(category);
        }

        [HttpPost]

        public async Task<ActionResult> CreateCategoryAsync(Category category)
        {
            if (category is null)
            {
                return BadRequest();
            }
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = category.ID }, category);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategoryAsync(int id, Category category)
        {
            if (id != category.ID)
            {
                return BadRequest();
            }

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory is null)
            {
                return NotFound("Category not found");
            }
            existingCategory.Name = category.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
            {
                return NotFound("Category not found");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
