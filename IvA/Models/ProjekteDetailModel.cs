using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class ProjekteDetailModel
    {
        public ProjekteModel Project { get; set; }
        public List<ArbeitsPaketModel> Packages { get; set; }
        public List<IdentityUser> ProjectUsers { get; set; }
    }
}
