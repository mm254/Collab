using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class ProjectRoles
    {
        [Key]
        public int ProjectRolesId { get; set; }

        public int ProjectId { get; set; }

        public string UserId { get; set; }

        public string ProjectRole { get; set; }
    }
}
