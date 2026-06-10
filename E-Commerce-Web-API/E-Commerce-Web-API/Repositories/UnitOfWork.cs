using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Interfaces.Repositories;

namespace E_Commerce_Web_API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ICategoryRepository Categories { get; private set; }

        public IProductRepository Products { get; private set; }

        public IOrderRepository Orders { get; private set; }

        public IOrderItemRepository OrderItems { get; private set; }

        public IStockRepository Stocks { get; private set; }

        public IReviewRepository Reviews { get; private set; }
        public UnitOfWork(AppDbContext context, ICategoryRepository categoryRepository
            , IProductRepository productRepository, IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository, IStockRepository stockRepository,
            IReviewRepository reviewRepository)
        {
            _context = context;
            Categories = categoryRepository;
            Products = productRepository;
            Orders = orderRepository;
            OrderItems = orderItemRepository;
            Stocks = stockRepository;
            Reviews = reviewRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
