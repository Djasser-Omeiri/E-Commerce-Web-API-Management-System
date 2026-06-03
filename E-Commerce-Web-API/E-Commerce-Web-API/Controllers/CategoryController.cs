using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce_Web_API.DTOs;

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
        [ProducesResponseType<CategoryDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesAsync()
        {
            var categoriesDTOs = await _context.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                .Select(c => new CategoryDTO
                {
                    ID = c.ID,
                    Name = c.Name,
                    ProductsCount = c.Products.Count()
                }).ToListAsync();
            if (categoriesDTOs is null)
            {
                return NotFound("Categories not found");
            }

            return Ok(categoriesDTOs);
        }
        [HttpGet("{id}")]
        [ProducesResponseType<CategoryDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDTO>> GetCategoryByIdAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest("Invalid category ID");
            }
            var category = await _context.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                .Select(c => new CategoryDTO
                {
                    ID = c.ID,
                    Name = c.Name,
                    ProductsCount = c.Products.Count()
                }).FirstOrDefaultAsync(c => c.ID == id);
            if (category is null)
            {
                return NotFound("Category not found");
            }
            return Ok(category);
        }

        [HttpPost]
        [ProducesResponseType<CategoryDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<CategoryDTO>> CreateCategoryAsync(CreateCategoryDTO categorydto)
        {
            if (categorydto is null)
            {
                return BadRequest("Invalid category data");
            }
            var category = new Category
            {
                Name = categorydto.Name
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = category.ID }, category);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCategoryAsync(int id, CreateCategoryDTO categorydto)
        {
            if (id < 0)
            {
                return BadRequest("Invalid category ID");
            }

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory is null)
            {
                return NotFound("Category not found");
            }
            existingCategory.Name = categorydto.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
