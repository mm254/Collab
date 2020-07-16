using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IvA.Models
{
    public class ProjekteDetailModel
    {
        public ProjekteModel Project { get; set; }
        public List<ArbeitsPaketModel> Packages { get; set; }
        public List<IdentityUser> ProjectUsers { get; set; }
        public string[] ProjectProgress { get; set; }
        public List<ProjectRoles> Roles { get; set; }
    }
}
