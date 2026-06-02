using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs;
using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync()
        {
            var productsDTOs = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Select(p => new ProductDTO
                {
                    ID = p.ID,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name ?? string.Empty
                }).ToListAsync();
            if (productsDTOs is null)
            {
                return NotFound();
            }

            return Ok(productsDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductByIdAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var productDTO = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Select(p => new ProductDTO
                {
                    ID = p.ID,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name ?? string.Empty
                })
                .FirstOrDefaultAsync(p => p.ID == id);
            if (productDTO is null)
            {
                return NotFound("Product not found");
            }

            return Ok(productDTO);
        }
        [HttpPost]
        public async Task<ActionResult> CreateProductAsync(CreateProductDTO productdto)
        {
            if (productdto is null)
            {
                return BadRequest();
            }
            var product = new Product
            {
                Name = productdto.Name,
                Price = productdto.Price,
                Description = productdto.Description,
                CategoryID = productdto.CategoryID
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductByIdAsync), new { id = product.ID }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProductAsync(int id, Product product)
        {
            if (id != product.ID)
            {
                return BadRequest();
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct is null)
            {
                return NotFound("Product not found");
            }
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.CategoryID = product.CategoryID;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return NotFound("Product not found");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
