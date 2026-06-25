using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Web_API.DTOs.Auth
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 charachters long.")]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
