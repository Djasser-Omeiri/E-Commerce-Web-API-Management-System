using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Interfaces;

namespace E_Commerce_Web_API.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
