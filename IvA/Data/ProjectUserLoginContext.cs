using Microsoft.EntityFrameworkCore;
using IvA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Datenbankkontextklasse für das Datenmodell ProjectUsersLogin. Dient als Schnittstelle zwischen Modell und Datenbank und steuert das Datenbankschema.
// Stellt Funktionalitäten von EF-Core bereit.

namespace IvA.Data
{
    public class ProjectUserLoginContext : DbContext
    {
        public ProjectUserLoginContext(DbContextOptions<ProjectUserLoginContext> options)
            : base(options)
        {

        }

        public DbSet<ProjectUsersLogin> ProjectUserLoginContexts { get; set; }

    }
}
