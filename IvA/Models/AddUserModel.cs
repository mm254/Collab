using System.ComponentModel.DataAnnotations;

namespace IvA.Models
{
    public class AddUserModel
    {
        [Key]
        public string name { get; set; }
        public int id { get; set; }
    }
}
