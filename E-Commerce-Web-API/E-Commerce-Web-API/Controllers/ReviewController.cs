using E_Commerce_Web_API.DTOs.Review;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using E_Commerce_Web_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Commerce_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType<IEnumerable<ReviewDTO>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviewsAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            string? filterUserId = isAdmin ? null : userId;

            var reviews = await _reviewService.GetReviewsFilterAsync(filterUserId);
            if (reviews == null)
            {
                _logger.LogWarning("No reviews found for user: {UserId}", userId);
                return NotFound("No reviews found");
            }

            return Ok(reviews);
        }

        [HttpGet("{id}")]
        [ProducesResponseType<ReviewDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewDTO>> GetReviewByIdAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                _logger.LogWarning("Review not found. ID: {ReviewId} by user: {UserId}", id, userId);
                return NotFound("Review not found");
            }

            if (!User.CanAccess(review.User.ID))
            {
                _logger.LogWarning("Access denied to review ID: {ReviewId} by user: {UserId}", id, userId);
                return Forbid();
            }

            return Ok(review);
        }

        [HttpGet("product/{productId}")]
        [ProducesResponseType<IEnumerable<ReviewDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviewsByProductIdAsync(int productId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
                _logger.LogInformation("Reviews retrieved successfully for product ID: {ProductId}", productId);
                return Ok(reviews);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("GetReviewsByProductId error for product ID: {ProductId}. Error: {Error}", productId, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType<Review>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Review>> CreateReviewAsync([FromBody] CreateReviewDTO reviewDTO)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (User.IsInRole("Admin"))
                {
                    _logger.LogInformation("CreateReview called by Admin: {AdminId} at {Time}", userId, DateTime.UtcNow);
                }

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("CreateReview failed - user ID not found in token. User: {UserId} at {Time}", userId, DateTime.UtcNow);
                    return Unauthorized("User ID not found in token");
                }

                var review = await _reviewService.CreateReviewAsync(reviewDTO, userId);
                return CreatedAtAction(nameof(GetReviewByIdAsync), new { id = review.ID }, review);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("CreateReview argument error. Error: {Error} at {Time}", ex.Message, DateTime.UtcNow);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateReviewAsync(int id, [FromBody] UpdateReviewDTO reviewDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Admin"))
            {
                _logger.LogInformation("UpdateReview called for review ID: {ReviewId} by Admin: {AdminId} at {Time}", id, userId, DateTime.UtcNow);
            }
            if (id <= 0)
            {
                _logger.LogWarning("UpdateReview failed - invalid review ID: {ReviewId}, User: {UserId} at {Time}", id, userId, DateTime.UtcNow);
                return BadRequest("Invalid Review ID");
            }
            var existingReview = await _reviewService.GetReviewByIdAsync(id);
            if (existingReview == null)
            {
                _logger.LogWarning("UpdateReview failed - review not found. ID: {ReviewId}, User: {UserId} at {Time}", id, userId, DateTime.UtcNow);
                return NotFound("Review not found");
            }
            if (!User.CanAccess(existingReview.User.ID))
            {
                _logger.LogWarning("Access denied for UpdateReview. ReviewId: {ReviewId}, User: {UserId} at {Time}", id, userId, DateTime.UtcNow);
                return Forbid();
            }
            try
            {
                var success = await _reviewService.UpdateReviewAsync(id, reviewDTO);
                if (!success)
                {
                    _logger.LogWarning("UpdateReview failed for review ID: {ReviewId} at {Time}", id, DateTime.UtcNow);
                    return NotFound();
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("UpdateReview argument error for review ID: {ReviewId}. Error: {Error} at {Time}", id, ex.Message, DateTime.UtcNow);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteReviewAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("Admin"))
            {
                _logger.LogInformation("DeleteReview called for review ID: {ReviewId} by Admin: {AdminId} at {Time}", id, userId, DateTime.UtcNow);
            }

            var review = await _reviewService.GetReviewEntityByIdAsync(id);
            if (review == null)
            {
                _logger.LogWarning("DeleteReview failed - review not found. ID: {ReviewId}, User: {UserId} at {Time}", id, userId, DateTime.UtcNow);
                return NotFound("Review not found");
            }

            if (!User.CanAccess(review.UserId))
            {
                _logger.LogWarning("Access denied for DeleteReview. ReviewId: {ReviewId}, User: {UserId} at {Time}", id, userId, DateTime.UtcNow);
                return Forbid();
            }

            var success = await _reviewService.DeleteReviewAsync(review);
            if (!success)
            {
                _logger.LogWarning("DeleteReview failed for review ID: {ReviewId} at {Time}", id, DateTime.UtcNow);
                return NotFound();
            }

            return NoContent();
        }
    }
}
