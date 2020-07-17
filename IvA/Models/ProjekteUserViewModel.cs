using System.ComponentModel.DataAnnotations;

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
