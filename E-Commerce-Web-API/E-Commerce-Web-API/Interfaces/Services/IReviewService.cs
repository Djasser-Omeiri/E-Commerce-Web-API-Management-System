using E_Commerce_Web_API.DTOs.Review;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetReviewsAsync();
        Task<ReviewDTO?> GetReviewByIdAsync(int id);
        Task<Review?> GetReviewEntityByIdAsync(int id);
        Task<Review> CreateReviewAsync(CreateReviewDTO reviewDTO, string userId);
        Task<bool> UpdateReviewAsync(int id, UpdateReviewDTO reviewDTO);
        Task<bool> DeleteReviewAsync(Review review);
        Task<IEnumerable<ReviewDTO>> GetReviewsByProductIdAsync(int productId);
    }
}
