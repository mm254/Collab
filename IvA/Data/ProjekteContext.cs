using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IvA.Models;
using Microsoft.EntityFrameworkCore;

namespace IvA.Data
{
    public class ProjekteContext : DbContext
    {
        public ProjekteContext(DbContextOptions<ProjekteContext> options)
                    : base(options)
        {
        }

        public DbSet<Projekte> Projekte { get; set; }
    }
}
