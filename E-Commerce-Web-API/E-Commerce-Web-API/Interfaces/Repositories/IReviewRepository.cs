using E_Commerce_Web_API.DTOs.Review;
using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task<IEnumerable<ReviewDTO>> GetReviewsAsync();
        Task<ReviewDTO?> GetReviewByIdAsync(int id);
        Task<Review?> GetReviewEntityByIdAsync(int id);
        Task<Review> CreateReviewAsync(CreateReviewDTO ReviewDTO);
        Task DeleteReviewAsync(Review Review);
    }
}
