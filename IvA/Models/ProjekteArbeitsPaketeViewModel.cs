using System.ComponentModel.DataAnnotations;

namespace IvA.Models
{
    /*
     ViewModel, um gleichzeitig Daten von Projekten und Arbeitspaketen an eine View zu übergeben.
     */
    public class ProjekteArbeitsPaketeViewModel
    {
        [Key]
        public int ProjekteArbeitsPaketeViewModelId { get; set; }
        public int ProjekteId { get; set; }
        public int ArbeitsPaketId { get; set; }

    }
}
