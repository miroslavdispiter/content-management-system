using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace content_management_system.Models
{
    public class User
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public UserRole Role { get; set; }

        public User() { }

        public User(string username, string password, UserRole userRole)
        {
            Username = username;
            Password = password;
            Role = userRole;
        }
    }
}
