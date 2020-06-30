using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class AddUserModel
    {
        [Key]
        public string name { get; set; }
        public int id { get; set; }
    }
}
