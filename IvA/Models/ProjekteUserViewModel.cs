using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class ProjekteUserViewModel
    {
        [Key]
        public int ProjekteUserViewModelId { get; set; }

        public int ProjekteId { get; set; }

        public int UserId { get; set; }
    }
}
