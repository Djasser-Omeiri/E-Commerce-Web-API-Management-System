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
        [ProducesResponseType<ProductDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsAsync()
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
                return NotFound("Products not found");
            }

            return Ok(productsDTOs);
        }

        [HttpGet("{id}")]
        [ProducesResponseType<ProductDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> GetProductByIdAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest("Invalid product ID");
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
        [ProducesResponseType<ProductDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDTO>> CreateProductAsync(CreateProductDTO productdto)
        {
            if (productdto is null)
            {
                return BadRequest("Invalid product data");
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProductAsync(int id, CreateProductDTO productdto)
        {
            if (id < 0)
            {
                return BadRequest("Invalid product ID");
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct is null)
            {
                return NotFound("Product not found");
            }
            existingProduct.Name = productdto.Name;
            existingProduct.Description = productdto.Description;
            existingProduct.Price = productdto.Price;
            existingProduct.CategoryID = productdto.CategoryID;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
