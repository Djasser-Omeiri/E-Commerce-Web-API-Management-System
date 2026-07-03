using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Web_API.DTOs.Review
{
    public class UpdateReviewDTO
    {
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
