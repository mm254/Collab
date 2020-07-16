using System.ComponentModel.DataAnnotations;

namespace IvA.Models
{
    public class UserLogin
    {
        [Key]
        public int UserLoginID { get; set; }

        public string UserName { get; set; }
        public string UserID { get; set; }
    }
}
