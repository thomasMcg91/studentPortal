
using System.Collections.Generic;

namespace SMS.Core.Models
{
    // Class representing a table in our database
    public class Student
    {
        public Student()
        {
            // initialise the Tickets relationship
            Tickets = new List<Ticket>();

            // intialise the StudentModules relationship
            StudentModules = new List<StudentModule>();

            // initialise the profile - we want student created with a profile
            Profile = new Profile { Grade = 0.0 };
        }

        // An int field named Id will by convention be used as primary key
        public int Id { get; set; }

        public string Name { get; set; }

        public string Course { get; set; }

        public int Age { get; set; }

        public string Email { get; set; }

        // Navigation properties 
        public Profile Profile { get; set; }
        public ICollection<Ticket> Tickets {get; set;}
        public ICollection<StudentModule> StudentModules {get; set;}
         
    }
}