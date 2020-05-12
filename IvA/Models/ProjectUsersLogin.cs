using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace IvA.Models
{
    public class ProjectUsersLogin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string References { get; set; }
        public string EnterpriseName { get; set; }
        public string Mail { get; set; }
        public string UserPassword {get; set;}

        private bool permission {get; set;}

    }
}
