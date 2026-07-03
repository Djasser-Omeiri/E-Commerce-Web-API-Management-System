using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce_Web_API.Interfaces.Services
{
    public interface ITokenService
    {
        string createToken(User user, string role);
    }
}
