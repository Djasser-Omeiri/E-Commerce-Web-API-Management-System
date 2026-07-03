using E_Commerce_Web_API.DTOs.Auth;
using E_Commerce_Web_API.DTOs.Login;
using E_Commerce_Web_API.Interfaces.Services;
using E_Commerce_Web_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            string primaryRole = roles.Count > 0 ? roles[0] : "User";

            return Ok(new
            {
                Username = user.UserName,
                Role = primaryRole,
                token = _token.createToken(user, primaryRole)
            });
        }

        [HttpGet("Profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyProfile()
        {
            string username = User.Identity.Name;

            string id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(new { Name = username, Id = id });
        }
    }
}
