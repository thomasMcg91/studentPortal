
using System;
using SMS.Core.Models;

namespace SMS.Data.Services
{
    public static class StudentServiceSeeder
    {
        public static void Seed(IStudentService svc)
        {
            // re-initialise the database then populate with seed data
            svc.Initialise();     

            // Create four students with related profiles
            var s1 = svc.AddStudent(new Student { Name = "Homer", Course = "COM741",   Age = 45, Email = "s1@email.com", Profile = new Profile { Grade = 0.0 } });   
            var s2 = svc.AddStudent(new Student { Name = "Marge", Course = "COM741",   Age = 40, Email = "s2@email.com", Profile = new Profile { Grade = 0.0 } });
            var s3 = svc.AddStudent(new Student { Name = "Bart",  Course = "Sleeping", Age = 13, Email = "s3@email.com", Profile = new Profile { Grade = 0.0 } });
            var s4 = svc.AddStudent(new Student { Name = "Lisa",  Course = "Maths",    Age = 10, Email = "s4@email.com", Profile = new Profile { Grade = 0.0 } });

            // create three modules
            var m1 = svc.AddModule( new Module { Title = "Computing" });
            var m2 = svc.AddModule( new Module { Title = "Maths" });
            var m3 = svc.AddModule( new Module { Title = "English" });
            
            // Add three tickets for Homer
            var t1 = svc.CreateTicket(s1.Id, "I need some Beer" );
            var t2 = svc.CreateTicket(s1.Id, "Bart you little ..." );
            var t3 = svc.CreateTicket(s1.Id, "Which buttton stops a nuclear meltdown?" );

            // Add two tickets for Bart
            var t4 = svc.CreateTicket(s3.Id, "How do i get out of doing any work" ); 
            var t5 = svc.CreateTicket(s3.Id, "Go to skinners office" );

            // Add one ticket for Lisa
            var t6 = svc.CreateTicket(s4.Id, "I need more work..");

            // Add a Module to bart 
            svc.AddStudentToModule(s3.Id, m3.Id);
            // Update module grade and recalculate overall grade
            svc.UpdateStudentModuleGrade(s3.Id, m3.Id, 50);
            svc.RecalculateStudentGrade(s3.Id);

            // Add three modules to Lisa
            svc.AddStudentToModule( s4.Id, m1.Id );
            svc.AddStudentToModule( s4.Id, m2.Id );
            svc.AddStudentToModule( s4.Id, m3.Id );
            // Update module grades and recalculate overall grade
            svc.UpdateStudentModuleGrade(s4.Id, m1.Id, 80);
            svc.UpdateStudentModuleGrade(s4.Id, m2.Id, 70);
            svc.UpdateStudentModuleGrade(s4.Id, m3.Id, 78);
            svc.RecalculateStudentGrade(s4.Id);

            // add users
            var u1 = svc.RegisterUser("guest", "guest", Role.Guest);
            var u2 = svc.RegisterUser("admin", "admin", Role.Admin);
            var u3 = svc.RegisterUser("manager", "manager", Role.Manager);


        }


    }


}