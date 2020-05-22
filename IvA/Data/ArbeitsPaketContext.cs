using IvA.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Data
{
    public class ArbeitsPaketContext : DbContext
    {
        public ArbeitsPaketContext(DbContextOptions<ArbeitsPaketContext> options)
                    : base(options)
        {
        }

        public ArbeitsPaketContext() 
        { 
        }

        public DbSet<ArbeitsPaket> ArbeitsPaket { get; set; }
    }
}
