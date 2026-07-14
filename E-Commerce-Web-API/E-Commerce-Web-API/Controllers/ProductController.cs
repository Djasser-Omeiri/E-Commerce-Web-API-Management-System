using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs.Product;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Commerce_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
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
                _logger.LogWarning("No products found");
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
                _logger.LogWarning("Invalid product ID requested: {ProductId}", id);
                return BadRequest("Invalid product ID");
            }
            var productDTO = await _productService.GetProductByIdAsync(id);
            if (productDTO is null)
            {
                _logger.LogWarning("Product not found. ID: {ProductId}", id);
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
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("CreateProductAsync called by admin: {AdminId}. Product: {ProductName} at {Time}", adminId, productdto.Name, DateTime.UtcNow);
            if (productdto is null)
            {
                _logger.LogWarning("CreateProductAsync failed - invalid product data. Admin: {AdminId} at {Time}", adminId, DateTime.UtcNow);
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
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("UpdateProductAsync called by admin: {AdminId} for product ID: {ProductId} at {Time}", adminId, id, DateTime.UtcNow);
            if (id < 0)
            {
                _logger.LogWarning("UpdateProductAsync failed - invalid product ID: {ProductId}, Admin: {AdminId} at {Time}", id, adminId, DateTime.UtcNow);
                return BadRequest("Invalid product ID");
            }

            try
            {
                var updated = await _productService.UpdateProductAsync(id, productdto);
                if (!updated)
                {
                    _logger.LogWarning("UpdateProductAsync failed - product not found. ID: {ProductId}, Admin: {AdminId} at {Time}", id, adminId, DateTime.UtcNow);
                    return NotFound("Product not found");
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("UpdateProductAsync error - {Error}, ID: {ProductId}, Admin: {AdminId} at {Time}", ex.Message, id, adminId, DateTime.UtcNow);
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
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("DeleteProductAsync called by admin: {AdminId} for product ID: {ProductId} at {Time}", adminId, id, DateTime.UtcNow);
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("DeleteProductAsync failed - product not found. ID: {ProductId}, Admin: {AdminId} at {Time}", id, adminId, DateTime.UtcNow);
                return NotFound("Product not found");
            }

            return NoContent();
        }
    }
}
