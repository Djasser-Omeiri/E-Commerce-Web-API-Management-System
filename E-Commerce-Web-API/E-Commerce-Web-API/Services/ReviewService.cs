using E_Commerce_Web_API.DTOs.Review;
using E_Commerce_Web_API.DTOs.User;
using E_Commerce_Web_API.Interfaces;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Review> CreateReviewAsync(CreateReviewDTO reviewDTO, string userId)
        {
            var product = await _unitOfWork.Products.GetProductEntityByIdAsync(reviewDTO.ProductID);
            if (product == null)
                throw new ArgumentException("Product not found");

            var review = new Review
            {
                Comment = reviewDTO.Comment,
                Rating = reviewDTO.Rating,
                ProductID = reviewDTO.ProductID,
                UserId = userId
            };

            await _unitOfWork.Reviews.CreateReviewAsync(review);
            await _unitOfWork.CompleteAsync();
            return review;
        }

        public async Task<bool> DeleteReviewAsync(Review review)
        {
            await _unitOfWork.Reviews.DeleteReviewAsync(review);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<ReviewDTO?> GetReviewByIdAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetReviewByIdAsync(id);
            return review == null ? null : new ReviewDTO
            {
                ID = review.ID,
                Comment = review.Comment,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt,
                ProductName = review.Product?.Name ?? string.Empty,
                User = new UserDTO
                {
                    ID = review.User.Id,
                    Username = review.User.UserName!
                }
            };
        }

        public async Task<Review?> GetReviewEntityByIdAsync(int id)
        {
            return await _unitOfWork.Reviews.GetReviewEntityByIdAsync(id);
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsFilterAsync(string? userId = null)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsFilterAsync(userId);
            return reviews.Select(r => new ReviewDTO
            {
                ID = r.ID,
                Comment = r.Comment,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                ProductName = r.Product?.Name ?? string.Empty,
                User = new UserDTO
                {
                    ID = r.User.Id,
                    Username = r.User.UserName!
                }
            });
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByProductIdAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetProductEntityByIdAsync(productId);
            if (product == null)
                throw new ArgumentException("Product not found");

            var allReviews = await _unitOfWork.Reviews.GetReviewsFilterAsync();
            return allReviews
                .Where(r => r.ProductID == productId)
                .Select(r => new ReviewDTO
                {
                    ID = r.ID,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    ProductName = r.Product?.Name ?? string.Empty,
                    User = new UserDTO
                    {
                        ID = r.User.Id,
                        Username = r.User.UserName!
                    }
                });
        }

        public async Task<bool> UpdateReviewAsync(int id, UpdateReviewDTO reviewDTO)
        {
            var existing = await _unitOfWork.Reviews.GetReviewEntityByIdAsync(id);
            if (existing == null)
                return false;

            existing.Comment = reviewDTO.Comment;
            existing.Rating = reviewDTO.Rating;

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
