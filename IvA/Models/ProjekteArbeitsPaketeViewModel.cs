using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class ProjekteArbeitsPaketeViewModel
    {
        [Key]
        public int ProjekteArbeitsPaketeViewModelId { get; set; }
        public int ProjekteId { get; set; }
        public int ArbeitsPaketId { get; set; }

    }
}
