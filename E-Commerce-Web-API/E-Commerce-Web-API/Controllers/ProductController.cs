using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs;
using E_Commerce_Web_API.Interfaces;
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
        private readonly IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        [ProducesResponseType<ProductDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsAsync()
        {
           var productsDTOs = await _productRepository.GetProductsAsync();
            if (productsDTOs is null)
            {
                return NotFound("Products not found");
            }

            return Ok(productsDTOs);
        }

        [HttpGet("{id}", Name = nameof(GetProductByIdAsync))]
        [ProducesResponseType<ProductDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> GetProductByIdAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest("Invalid product ID");
            }
            var productDTO = await _productRepository.GetProductByIdAsync(id);
            if (productDTO is null)
            {
                return NotFound("Product not found");
            }

            return Ok(productDTO);
        }
        [HttpPost]
        [ProducesResponseType<ProductDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> CreateProductAsync(CreateProductDTO productdto)
        {
            if (productdto is null)
            {
                return BadRequest("Invalid product data");
            }
            var product = await _productRepository.CreateProductAsync(productdto);

            return CreatedAtRoute(nameof(GetProductByIdAsync), new { id = product.ID }, product);
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

            var existingProduct = await _productRepository.GetProductEntityByIdAsync(id);
            if (existingProduct is null)
            {
                return NotFound("Product not found");
            }
            existingProduct.Name = productdto.Name;
            existingProduct.Description = productdto.Description;
            existingProduct.Price = productdto.Price;
            existingProduct.CategoryID = productdto.CategoryID;

            await _productRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetProductEntityByIdAsync(id);
            if (product is null)
            {
                return NotFound("Product not found");
            }

            await _productRepository.DeleteProductAsync(product);
            return NoContent();
        }
    }
}
