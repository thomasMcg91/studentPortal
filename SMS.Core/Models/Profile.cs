
using System;

namespace SMS.Core.Models
{  
    // Class representing a table in our database
    public class Profile
    {
        public int Id { get; set; }

        public double Grade { get; set; }

        // Foreign Key attribute - convention is it begins with the name of the
        // related model and ends with Id
        public int StudentId { get; set; }

        // Navigation property to navigate to the related Student
        public Student Student { get; set; }

    }
}