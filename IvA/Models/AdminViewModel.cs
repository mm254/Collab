using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IvA.Models
{
    public class AdminViewModel
    {
        [Key]
        public int id { get; set; }
        public IdentityUser user { get; set; }

        public IdentityRole role { get; set; }
    }
}