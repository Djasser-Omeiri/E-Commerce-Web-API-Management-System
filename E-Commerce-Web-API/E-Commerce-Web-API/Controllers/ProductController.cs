using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs.Product;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType<IEnumerable<ProductDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsAsync()
        {
            var productsDTOs = await _productService.GetProductsAsync();
            if (productsDTOs is null)
            {
                return NotFound("Products not found");
            }

            return Ok(productsDTOs);
        }

        [HttpGet("{id}", Name = nameof(GetProductByIdAsync))]
        [AllowAnonymous]
        [ProducesResponseType<ProductDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> GetProductByIdAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest("Invalid product ID");
            }
            var productDTO = await _productService.GetProductByIdAsync(id);
            if (productDTO is null)
            {
                return NotFound("Product not found");
            }

            return Ok(productDTO);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType<Product>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Product>> CreateProductAsync(CreateProductDTO productdto)
        {
            if (productdto is null)
            {
                return BadRequest("Invalid product data");
            }
            var product = await _productService.CreateProductAsync(productdto);

            return CreatedAtRoute(nameof(GetProductByIdAsync), new { id = product.ID }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateProductAsync(int id, UpdateProductDTO productdto)
        {
            if (id < 0)
                return BadRequest("Invalid product ID");

            try
            {
                var updated = await _productService.UpdateProductAsync(id, productdto);
                if (!updated)
                    return NotFound("Product not found");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProductAsync(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound("Product not found");

            return NoContent();
        }
    }
}
