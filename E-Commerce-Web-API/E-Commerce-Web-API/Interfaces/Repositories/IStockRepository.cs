using E_Commerce_Web_API.DTOs.Stock;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Repositories
{
    public interface IStockRepository
    {
        Task<IEnumerable<StockDTO>> GetStocksAsync();
        Task<StockDTO?> GetStockByIdAsync(int id);
        Task<Stock?> GetStockEntityByIdAsync(int id);
        Task<Stock> CreateStockAsync(CreateStockDTO StockDTO);
        Task DeleteStockAsync(Stock Stock);
    }
}
