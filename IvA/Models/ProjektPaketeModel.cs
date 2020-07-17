using System.ComponentModel.DataAnnotations;

namespace IvA.Models
{
    /*
     ViewModel, um gleichzeitig Daten von Projekten, Arbeitspaketen und der gekreuzten Tablle ProjektePakete an eine View zu übergeben.
     */
    public class ProjektPaketeModel
    {
        [Key]
        public int id { get; set; }
        public ProjekteArbeitsPaketeViewModel ProjektPakete { get; set; }
        public ArbeitsPaketModel Pakete { get; set; }
        public ProjekteModel Projekte { get; set; }
    }
}
