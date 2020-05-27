using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IvA.Models;

namespace IvA.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<IvA.Models.Role> Role { get; set; }

        public DbSet<Projekte> Projekte { get; set; }
        public DbSet<ArbeitsPaket> ArbeitsPaket { get; set; }

        public DbSet<ProjekteArbeitsPaketeViewModel> ProjekteArbeitsPaketeViewModel { get; set; }

        public DbSet<IvA.Models.ProjektPaketeModel> ProjektPaketeModel { get; set; }
    }
}
