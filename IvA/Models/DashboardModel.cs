using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class DashboardModel
    {
        public PaketeUserViewModel UserPackages { get; set; }
        public ArbeitsPaketModel Packages { get; set; }
    }
}
