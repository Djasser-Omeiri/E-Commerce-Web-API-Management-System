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

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [ProducesResponseType<IEnumerable<ReviewDTO>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReviews()
        {
            string? filterUserId = User.IsInRole("Admin")
                ? null
                : User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reviews = await _reviewService.GetReviewsFilterAsync(filterUserId);
            if (reviews == null)
            {
                return NotFound("No reviews found");
            }

            return Ok(reviews);
        }

        [HttpGet("{id}")]
        [ProducesResponseType<ReviewDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound("Review not found");

            if (!User.CanAccess(review.User.ID))
                return Forbid();

            return Ok(review);
        }

        [HttpGet("product/{productId}")]
        [ProducesResponseType<IEnumerable<ReviewDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetReviewsByProductId(int productId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
                return Ok(reviews);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType<Review>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO reviewDTO)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                var review = await _reviewService.CreateReviewAsync(reviewDTO, userId);
                return CreatedAtAction(nameof(GetReviewById), new { id = review.ID }, review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDTO reviewDTO)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Review ID");
            }
            var existingReview = await _reviewService.GetReviewByIdAsync(id);
            if (existingReview == null)
                return NotFound("Review not found");
            if (!User.CanAccess(existingReview.User.ID))
            {
                return Forbid();
            }
            try
            {
                var success = await _reviewService.UpdateReviewAsync(id, reviewDTO);
                if (!success)
                    return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _reviewService.GetReviewEntityByIdAsync(id);
            if (review == null)
                return NotFound("Review not found");

            if (!User.CanAccess(review.UserId))
            {
                return Forbid();
            }

            var success = await _reviewService.DeleteReviewAsync(review);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}
