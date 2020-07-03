using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IvA.Models;

namespace IvA.Data
{

    /*
    Die Datenbankkontextklasse stellt Entitiy-Framwork Funktionen für ein Datenmodell bereit. Da nur eine Datenbank eingesetzt wird, verwenden wir für alle Datenmodelle
    eine zentrale Kontexklassse. DbSet<> erstellt eine Instanz von DbSet und stellt die Entity-Operationen zur Verfügung.
    Der Unterordner Migrationen enthält alle Datenbankmigrationen, mit deren Hilfe Tabellen generiert und verändert wurden.
     */
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<IvA.Models.RoleModel> Role { get; set; }

        public DbSet<ProjekteModel> Projekte { get; set; }
        public DbSet<ArbeitsPaketModel> ArbeitsPaket { get; set; }

        public DbSet<ProjekteArbeitsPaketeViewModel> ProjekteArbeitsPaketeViewModel { get; set; }

        public DbSet<IvA.Models.ProjektPaketeModel> ProjektPaketeModel { get; set; }

        public DbSet<ProjekteUserViewModel> ProjekteUserViewModel { get; set; }

        public DbSet<PaketeUserViewModel> PaketeUserViewModel { get; set; }

        public DbSet<UserLogin> UserLogin { get; set; }

        public DbSet<Message> Message { get; set; }

        public DbSet<UserLoginMessageViewModel> UserLoginMessageViewModel { get; set; }

        public DbSet<IvA.Models.AddUserModel> AddUserModel { get; set; }

        public DbSet<ProjectRoles> ProjectRoles { get; set; }
    }
}
