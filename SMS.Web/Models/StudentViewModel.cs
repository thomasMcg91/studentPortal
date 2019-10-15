using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Core.Models;

namespace SMS.Web.Models
{
    public class StudentViewModel
    {

        public StudentViewModel()
        {
            // initialise the Tickets relationship
            Tickets = new List<Ticket>();

            // intialise the StudentModules relationship
            StudentModules = new List<StudentModule>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Course { get; set; }

        public int Age { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public double Grade { get; set; }
           
        // Navigation properties        
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<StudentModule> StudentModules { get; set; }

        // Readonly helper properties
        public string Classification => ToClassification(Grade);
        public bool HasNoStudentModules => StudentModules.Count == 0;
        public bool HasStudentModules => !HasNoStudentModules;

        // convert to/from student
        public Student ToStudent()
        {
            return new Student
            {
                Id = this.Id,
                Name = this.Name,
                Course = this.Course,
                Age = this.Age,
                Email = this.Email
            };
        }

        public static StudentViewModel FromStudent(Student s)
        {
            return new StudentViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Course = s.Course,
                Age = s.Age,
                Email = s.Email,
                Grade = s.Profile.Grade,               
                Tickets = s.Tickets,
                StudentModules = s.StudentModules
            };
        }

        // private utility method to convert grade to Classification
        private string ToClassification(double grade)
        {
            switch (grade)
            {
                case double s when s >= 70: return "Distinction";
                case double s when s >= 60: return "Commendation";
                case double s when s >= 50: return "Pass";
                default: return "Fail";
            }
        }
    }

}
