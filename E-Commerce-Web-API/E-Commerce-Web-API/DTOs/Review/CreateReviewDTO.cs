namespace E_Commerce_Web_API.DTOs.Review
{
    public class CreateReviewDTO
    {
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public int ProductID { get; set; }

    }
}
