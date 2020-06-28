using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class ProjektPaketeModel
    {
        [Key]
        public int id { get; set; }
        public ProjekteArbeitsPaketeViewModel ProjektPakete { get; set; }
        public ArbeitsPaketModel Pakete { get; set; }
        public ProjekteModel Projekte { get; set; }
        //public List<IdentityUser> Users { get; set; }
    }
}
