using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IvA.Models
{
    public class PackagesDetailModel
    {
        public ArbeitsPaketModel Package { get; set; }
        public List<IdentityUser> ProjectUsers { get; set; }
        public List<IdentityUser> PackageUsers { get; set; }
        public List<ProjectRoles> Roles { get; set; }
    }
}
