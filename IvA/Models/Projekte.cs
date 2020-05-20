using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IvA.Areas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace IvA.Models
{
    public class Projekte {

        public int Id { get; set; }
        public string Projektname { get; set; }
        public string Projektersteller { get; set; }

        public static DateTime ErstelltAm { get; set; } = DateTime.Now;

        public string Mitglieder { get; set; }

        public string Beschreibung { get; set; }

        public DateTime Deadline { get; set; } = ErstelltAm.AddDays(30);

        public string Status { get; set; } = "To Do";


    }
}
