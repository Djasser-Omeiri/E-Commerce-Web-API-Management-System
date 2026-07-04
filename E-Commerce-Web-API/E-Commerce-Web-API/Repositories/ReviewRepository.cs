using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.Interfaces.Repositories;
using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_API.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            return review;
        }

        public async Task DeleteReviewAsync(Review review)
        {
            _context.Reviews.Remove(review);
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ID == id);
        }

        public async Task<Review?> GetReviewEntityByIdAsync(int id)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.ID == id);
        }

        public async Task<IEnumerable<Review>> GetReviewsFilterAsync(string? userId = null)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(o => userId == null || o.UserId == userId)
                .ToListAsync();
        }
    }
}

