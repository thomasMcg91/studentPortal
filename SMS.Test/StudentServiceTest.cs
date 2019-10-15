using System;
using Xunit;
using SMS.Data.Services;
using SMS.Core.Models;

namespace SMS.Test
{
    public class StudentServiceTest
    {
        private readonly IStudentService svc;

        public StudentServiceTest()
        {
            // general arrangement
            svc = new StudentService();
            svc.Initialise();
        }

        [Fact]
        public void GetStudents_NewDb_ShouldReturn0()
        {
            // act 
            var students = svc.GetStudents();
            var count = students.Count;

            // assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetStudents_With2Added_ShouldReturn2()
        {
            // arrange
            svc.AddStudent(new Student { Name = "XXX", Email = "xxx@email.com" , Course = "CCC" });
            svc.AddStudent(new Student { Name = "YYY", Email = "yyy@email.com" , Course = "CCC" });

            // act            
            var count = svc.StudentCount();

            // assert
            Assert.Equal(2, count);
        }


        [Fact]
        public void GetStudent_NewDb_ShouldReturnNull()
        {
            // act 
            var student = svc.GetStudent(1); // non existent student

            // assert
            Assert.Null(student);
        }

        [Fact]
        public void AddStudent_NewDb_ShouldExist()
        {
            // act 
            var s = svc.AddStudent(new Student { Name = "XXX", Email = "xxx@email.com", Course = "CCC" });
            var ns = svc.GetStudent(s.Id);

            // assert
            Assert.Equal(s.Id, ns.Id);
        }

        [Fact]
        public void UpdateStudent_InClonedState_ShouldUpdate()
        {
            // act - add a new student
            var s = svc.AddStudent(new Student { Name = "XXX", Email = "xxx@email.com", Course = "CCC" });

            // make a clone of the entity (s)
            var clone = new Student { Id = s.Id, Name = s.Name, Course = s.Course, Age = s.Age, Email = s.Email };
            // update the clone age
            clone.Age = 100;

            // update the database with the cloned version (disconnected)
            svc.UpdateStudent(clone);

            // assert - update worked
            Assert.Equal(100, clone.Age);
        }

        [Fact]
        public void DeleteStudent_ThatExists_ShouldWork()
        {
            // act 
            var s = svc.AddStudent(new Student { Name = "XXX", Email = "xxx@email.com" , Course = "CCC" });
            var deleted = svc.DeleteStudent(s.Id);

            // assert
            Assert.True(deleted);
        }

        [Fact]
        public void DeleteStudent_ThatDoesntExist_ShouldNotWork()
        {
            // act 	
            var deleted = svc.DeleteStudent(0);

            // assert
            Assert.False(deleted);
        }

        [Fact]
        public void AddStudentToModule_WhereNotAlreadyTakingModuleAndModuleExists_ShouldWork()
        {
            // arrange
            var s = svc.AddStudent(new Student { Name = "XXX", Email = "test@email.com" , Course = "CCC" });

            var m = svc.AddModule(new Module { Title = "XXXX" });

            // act
            var sm = svc.AddStudentToModule(s.Id, m.Id);
            var r = svc.GetStudent(s.Id);
            Assert.Equal(1, r.StudentModules.Count);
        }

          [Fact]
        public void UpdateProfileGrade_WhereTaking2Modules_ShouldSetGrade()
        {
            // arrange
            var s = svc.AddStudent(new Student { Name = "XXX", Email = "test@email.com" , Course = "CCC" });
            var m1 = svc.AddModule(new Module { Title = "M1" });
            var m2 = svc.AddModule(new Module { Title = "M2" });
            
                  
            // act - add these modules to the student 
            var sm1 = svc.AddStudentToModule(s.Id, m1.Id);
            var sm2 = svc.AddStudentToModule(s.Id, m2.Id);
            // update the module grades
            svc.UpdateStudentModuleGrade(s.Id, sm1.ModuleId, 40);
            svc.UpdateStudentModuleGrade(s.Id, sm2.ModuleId, 60);
            // recalculate the overall grade (should be 50)
            svc.RecalculateStudentGrade(s.Id);

            // get the updated student
            var r = svc.GetStudent(s.Id);

            Assert.Equal(50, r.Profile.Grade);
        }

        [Fact]
        public void CloseTicket_ForNonExistentTicket_ShouldReturnNull()
        {
            // act 	
            var ticket = svc.CloseTicket(0);

            // assert
            Assert.Null(ticket);
        }

        [Fact]
        public void CloseTicket_ForAlreadyClosedTicket_ShouldReturnNull()
        {
            // arrange
            StudentServiceSeeder.Seed(svc);

            // get first open tickets and check some exist
            var tickets = svc.GetActiveTickets();
            Assert.NotEmpty(tickets);
            // close first ticket
            var ticket = svc.CloseTicket(tickets[0].Id);
        
            // act 	
            var closed = svc.CloseTicket(ticket.Id);
            
            // assert
            Assert.Null(closed);
        }
        
    }
}
