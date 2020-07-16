using IvA.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace IvA.Models
{
    /*
     Datenmodell für ein Projekt
     */
    public class ProjekteModel
    {

        [Key]
        public int ProjekteId { get; set; }

        public int MitgliederId { get; set; }
        public string Projektname { get; set; }
        public string Projektersteller { get; set; }

        [Display(Name = "Erstellt:")]
        [DataType(DataType.Date)]
        public DateTime ErstelltAm { get; set; }

        public string Mitglieder { get; set; }

        public string Beschreibung { get; set; }

        [DataType(DataType.Date)]
        [CurrentDate(ErrorMessage = "Das Datum der Deadline muss in der Zukunft liegen!")]
        public DateTime Deadline { get; set; }

        public string Status { get; set; } = "To Do";




    }
}
