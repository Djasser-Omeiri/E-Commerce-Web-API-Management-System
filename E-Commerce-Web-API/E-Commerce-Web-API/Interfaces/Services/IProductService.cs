using E_Commerce_Web_API.DTOs.Product;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync();
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<Product?> GetProductEntityByIdAsync(int id);
        Task<Product> CreateProductAsync(CreateProductDTO productDTO);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
    }
}
