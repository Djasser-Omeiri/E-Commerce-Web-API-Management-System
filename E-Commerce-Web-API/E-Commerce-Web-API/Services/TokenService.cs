using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce_Web_API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string createToken(ApplicationUser user,string role)
        {
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.UserName!),
            new Claim(ClaimTypes.Role,role)
        };
            var authSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));
            var creds = new SigningCredentials(authSignKey, SecurityAlgorithms.HmacSha256);

            var tokenObject = new JwtSecurityToken(
               issuer: _configuration["JWT:Issuer"],
              audience: _configuration["JWT:Audience"],
              expires: DateTime.UtcNow.AddHours(3),
              claims: authClaims,
              signingCredentials: creds

                );
            return new JwtSecurityTokenHandler().WriteToken(tokenObject);
        }

    }
}
