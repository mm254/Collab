using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class Message
    {
        [Key]
        public int MessageID {get; set;}
        public string QuellID { get; set; }
        public string ZielID { get; set; }
        public string Nachricht { get; set; }
        public bool Status { get; set; }
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }
    }
}
