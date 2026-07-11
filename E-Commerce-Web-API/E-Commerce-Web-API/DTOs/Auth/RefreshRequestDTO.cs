namespace E_Commerce_Web_API.DTOs.Auth
{
    public class RefreshRequestDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
