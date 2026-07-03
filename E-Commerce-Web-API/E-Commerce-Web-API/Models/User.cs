using Microsoft.AspNetCore.Identity;

namespace E_Commerce_Web_API.Models
{
    public class User : IdentityUser
    {
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
