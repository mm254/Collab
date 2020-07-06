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
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
