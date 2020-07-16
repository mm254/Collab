using System.ComponentModel.DataAnnotations;

namespace IvA.Models
{
    public class RoleModel
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
