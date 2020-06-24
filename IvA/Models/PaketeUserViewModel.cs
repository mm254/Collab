using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class PaketeUserViewModel
    {
        [Key]
        public int PaketeUserViewModelId { get; set;}

        public int ArbeitsPaketId { get; set; }

        public string UserId { get; set; }
    }
}
