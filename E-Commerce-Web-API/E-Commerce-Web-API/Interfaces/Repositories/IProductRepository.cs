using E_Commerce_Web_API.DTOs.Product;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> GetProductEntityByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
    }
}
