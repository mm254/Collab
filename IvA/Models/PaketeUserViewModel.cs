using System.ComponentModel.DataAnnotations;

namespace IvA.Models
{
    /*
     ViewModel, um gleichzeitig Daten von Arbeitspaketen und Usern an eine View zu übermitteln
     */
    public class PaketeUserViewModel
    {
        [Key]
        public int PaketeUserViewModelId { get; set; }

        public int ArbeitsPaketId { get; set; }

        public string UserId { get; set; }
    }
}
