using E_Commerce_Web_API.Interfaces.Repositories;

namespace E_Commerce_Web_API.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        IStockRepository Stocks { get; }
        IReviewRepository Reviews { get; }
        Task<int> CompleteAsync();
    }
}
