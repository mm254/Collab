using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IvA.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }
    }
}