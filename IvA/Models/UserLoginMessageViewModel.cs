using System.ComponentModel.DataAnnotations;

namespace IvA.Models
{
    public class UserLoginMessageViewModel
    {
        [Key]
        public int UserLoginMessageViewModelID { get; set; }

        public int UserLoginID { get; set; }

        public int MessageID { get; set; }

    }
}
