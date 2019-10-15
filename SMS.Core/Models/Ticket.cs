
using System;

namespace SMS.Core.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Issue { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }

        public string Response { get; set; }

        // Foreign key relating to Student ticket owner
        public int StudentId { get; set; }

        // Navigation property to navigate to the student
        public Student Student { get; set; }
    } 
}
