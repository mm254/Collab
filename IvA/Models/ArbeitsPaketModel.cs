using IvA.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace IvA.Models
{
    /*
     Datenmodell für Arbeitspakete, welche innerhalb eines Projektes erstellt werden können.
     */
    public class ArbeitsPaketModel
    {
        [Key]
        public int ArbeitsPaketId { get; set; }

        public int ProjektId { get; set; }

        public string PaketName { get; set; }

        public string Beschreibung { get; set; }

        public string Mitglieder { get; set; }

        [DataType(DataType.Date)]
        [CurrentDate(ErrorMessage = "Das Datum der Frist muss in der Zukunft und vor Ablauf der Projektdeadline liegen!")]
        public DateTime Frist { get; set; }

        public string Status { get; set; }

        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Geben sie eine Ganzzahl ein")]
        public int Zeitbudget { get; set; }

        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Geben sie eine Ganzzahl ein")]
        public int VerbrauchteZeit { get; set; }

    }
}
