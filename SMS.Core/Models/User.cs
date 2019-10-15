using System;

namespace SMS.Core.Models
{
    public enum Role { Admin, Manager, Guest }

    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
