using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
