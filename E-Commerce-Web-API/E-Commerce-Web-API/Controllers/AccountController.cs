using E_Commerce_Web_API.DTOs.Auth;
using E_Commerce_Web_API.DTOs.Login;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_Commerce_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _token;

        public AccountController(UserManager<User> userManager, ITokenService token)
        {
            _userManager = userManager;
            _token = token;
        }
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterDTO UserFromRequest)
        {
            var user = new User
            {
                UserName = UserFromRequest.UserName,
                Email = UserFromRequest.Email
            };

            var result = await _userManager.CreateAsync(user, UserFromRequest.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddToRoleAsync(user, "User");

            return Ok("Account created successfully.");
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginDTO UserFromRequest)
        {
            var user = await _userManager.FindByNameAsync(UserFromRequest.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, UserFromRequest.Password))
            {
                return Unauthorized("Invalid credentials.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            string primaryRole = roles.FirstOrDefault() ?? "User";

            var accessToken = _token.createToken(user, primaryRole);
            var secureBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(secureBytes);
            }
            var refreshToken = Convert.ToBase64String(secureBytes);

            await _userManager.SetAuthenticationTokenAsync(
                user,
                loginProvider: "EcommerceAPI",
                tokenName: "RefreshToken",
                tokenValue: refreshToken);

            return Ok(new
            {
                Username = user.UserName,
                Role = primaryRole,
                token = accessToken,
                RefreshToken = refreshToken
            });
        }
        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh(RefreshRequestDTO request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user is null)
            {
                return Unauthorized("Invalid refresh request.");
            }

            var savedToken = await _userManager.GetAuthenticationTokenAsync(user, "EcommerceAPI", "RefreshToken");

            if (savedToken is null || savedToken != request.RefreshToken)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            string primaryRole = roles.FirstOrDefault() ?? "User";

            var newAccessToken = _token.createToken(user, primaryRole);

            var secureBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(secureBytes);
            }
            var newRefreshToken = Convert.ToBase64String(secureBytes);

            await _userManager.SetAuthenticationTokenAsync(user, "EcommerceAPI", "RefreshToken", newRefreshToken);

            return Ok(new
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout(LogoutRequestDTO request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null)
            {
                return Ok(new { Message = "Logged out successfully." });
            }

            var savedToken = await _userManager.GetAuthenticationTokenAsync(user, "EcommerceAPI", "RefreshToken");

            if (savedToken is not null && savedToken == request.RefreshToken)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "EcommerceAPI", "RefreshToken");
            }

            return Ok(new { Message = "Logged out successfully." });
        }

        [HttpGet("Profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProfile()
        {
            string username = User.Identity.Name;

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            return Ok(new { Name = username, Id = id });
        }
    }
}
