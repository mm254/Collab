using IvA.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class ArbeitsPaketModel
    {
        public int ArbeitsPaketId { get; set; }

        public string PaketName { get; set; }

        public string Beschreibung { get; set; }

        public string Mitglieder { get; set; }

        [DataType(DataType.Date)]
        [CurrentDate(ErrorMessage = "Das Datum der Frist muss in der Zukunft liegen!")]
        public DateTime Frist { get; set; }

        public string Status { get; set; }
    }
}
