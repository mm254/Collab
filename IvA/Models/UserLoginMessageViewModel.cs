using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
