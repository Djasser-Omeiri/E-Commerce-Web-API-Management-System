using E_Commerce_Web_API.Models;

namespace E_Commerce_Web_API.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetReviewsFilterAsync(string? userId = null);
        Task<Review?> GetReviewByIdAsync(int id);
        Task<Review?> GetReviewEntityByIdAsync(int id);
        Task<Review> CreateReviewAsync(Review review);
        Task DeleteReviewAsync(Review review);
    }
}
