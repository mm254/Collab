using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class UserLogin
    {
        /*public int v1;
        public string v2;
        public string v3;

        public UserLogin(int v1, string v2, string v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
        */
        [Key]
        public int UserLoginID { get; set; }

        public string UserName { get; set; }
        public string UserID { get; set; }

        //mio
        /*public UserLogin(int id, string name, string userid) {
            UserLoginID = id;
            UserName = name;
            UserID = userid;
        }
        */

    }
}
