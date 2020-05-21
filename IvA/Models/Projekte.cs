using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IvA.Areas;
using IvA.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace IvA.Models
{
    public class Projekte {

        public int Id { get; set; }
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
