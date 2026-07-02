using System.Security.Claims;

namespace E_Commerce_Web_API.Services
{
    public static class UserSecurityExtensions
    {
        //Ownership-Based Authorization
        public static bool CanAccess(this ClaimsPrincipal user, string resourceOwnerId)
        {
            if (user.IsInRole("Admin")) return true;

            var loggedInUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return loggedInUserId == resourceOwnerId;
        }
    }
}
