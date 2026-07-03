using E_Commerce_Web_API.DTOs.User;

namespace E_Commerce_Web_API.DTOs.Review
{
    public class ReviewDTO
    {
        public int ID { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public UserDTO User { get; set; } = new UserDTO();
    }
}
