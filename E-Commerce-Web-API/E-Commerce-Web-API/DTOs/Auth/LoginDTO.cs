using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Web_API.DTOs.Login
{
    public class LoginDTO
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
