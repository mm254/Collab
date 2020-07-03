using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    /*
     ViewModel, um gleichzeitg Daten von Projekten und Usern an eine View zu übergeben.
     */
    public class ProjekteUserViewModel
    {
        [Key]
        public int ProjekteUserViewModelId { get; set; }

        public int ProjekteId { get; set; }

        public string UserId { get; set; }
    }
}
