using E_Commerce_Web_API.Data;
using E_Commerce_Web_API.DTOs.Review;
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

        public async Task<Review> CreateReviewAsync(CreateReviewDTO ReviewDTO)
        {
            var Review = new Review
            {
                Comment = ReviewDTO.Comment,
                ProductID = ReviewDTO.ProductID,
                Rating = ReviewDTO.Rating
            };
            _context.Reviews.Add(Review);
            return Review;
        }

        public async Task DeleteReviewAsync(Review Review)
        {
            _context.Reviews.Remove(Review);
        }

        public async Task<ReviewDTO?> GetReviewByIdAsync(int id)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Select(r => new ReviewDTO
                {
                    ID = r.ID,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    ProductName = r.Product.Name
                }).FirstOrDefaultAsync(r => r.ID == id);
        }

        public async Task<Review?> GetReviewEntityByIdAsync(int id)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.ID == id);
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsAsync()
        {
            return await _context.Reviews
                .AsNoTracking()
                .Select(r => new ReviewDTO
                {
                    ID = r.ID,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    ProductName = r.Product.Name
                }).ToListAsync();
        }
    }
}
