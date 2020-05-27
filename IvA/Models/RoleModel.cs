using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IvA.Validation;

namespace IvA.Models
{
    public class RoleModel
    {
        [Key]
        public String Id { get; set; }
        public String Name { get; set; }
    }
}
