using E_Commerce_Web_API.DTOs.Product;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Product> CreateProductAsync(CreateProductDTO productDTO)
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Price = productDTO.Price,
                Description = productDTO.Description,
                CategoryID = productDTO.CategoryID,
                Stock = new Stock { Quantity = productDTO.InitialStock }
            };
            await _unitOfWork.Products.CreateProductAsync(product);
            await _unitOfWork.CompleteAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetProductEntityByIdAsync(id);
            if (product == null) return false;
            await _unitOfWork.Products.DeleteProductAsync(product);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetProductByIdAsync(id);
            if (product == null) return null;
            return new ProductDTO
            {
                ID = product.ID,
                Name = product.Name,
                Price = product.Price,
                CategoryName = product.Category.Name,
                IsAvailable = product.Stock?.Quantity > 0
            };
        }

        public async Task<Product?> GetProductEntityByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetProductEntityByIdAsync(id);
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDTO productDTO)
        {
            var existing = await _unitOfWork.Products.GetProductEntityByIdAsync(id);
            if (existing == null) return false;

            var category = await _unitOfWork.Categories.GetCategoryEntityByIdAsync(productDTO.CategoryID);
            if (category == null)
                throw new ArgumentException("Category not found");

            existing.Name = productDTO.Name;
            existing.Description = productDTO.Description;
            existing.Price = productDTO.Price;
            existing.CategoryID = productDTO.CategoryID;

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            var products = await _unitOfWork.Products.GetProductsAsync();
            return await products.Select(p => new ProductDTO
            {
                ID = p.ID,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name,
                IsAvailable = p.Stock.Quantity > 0
            }).ToListAsync();
        }
    }
}