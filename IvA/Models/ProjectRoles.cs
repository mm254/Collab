using System.ComponentModel.DataAnnotations;

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
