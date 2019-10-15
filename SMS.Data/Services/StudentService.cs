using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using SMS.Core.Models;
using SMS.Data.Repositories;

namespace SMS.Data.Services
{
    public class StudentService : IStudentService
    {
        private readonly DatabaseContext db;

        public StudentService()
        {
            db = new DatabaseContext();           
        }

        public void Initialise()
        {
            db.Initialise();
        }

        public Module AddModule(Module m)
        {
            db.Modules.Add(m);
            db.SaveChanges();
            return m;
        }

        public Module GetModule(int id)
        {
            return db.Modules.FirstOrDefault(s => s.Id == id);
        }

        public IList<Module> GetModules()
        {
            return db.Modules.ToList();
        }

        public Student GetStudent(int id)
        {
            return db.Students.Include(s => s.Profile)
                              .Include(s => s.Tickets)
                              .Include(s => s.StudentModules)
                              .ThenInclude(sm => sm.Module)                             
                              .FirstOrDefault(s => s.Id == id);
        }

        public Student AddStudent(Student s)
        {
            // here we could optionally check if a student with the same email exists
            // and if so return null, otherwise continue and add the new student             

            db.Students.Add(s);
            db.SaveChanges();
            return s;
        }

        public bool DeleteStudent(int id)
        {
            // verify the student exists
            var s = GetStudent(id);
            if (s == null)
            {
                return false; // no such student
            }
            // remove the student
            db.Students.Remove(s);
            db.SaveChanges();
            return true;
        }

        public Student UpdateStudent(Student s)
        {
            // verify that this student exists
            var o = GetStudent(s.Id);
            if (o == null)
            {
                return null;
            }

            // ** disconnect entity (o) from EF Change tracking so we **
            // ** can update the new entity (s) without a conflict    **
            db.Entry(o).State = EntityState.Detached;

            // tell EF that this entity has changed
            db.Update(s);

            // save the changes
            db.SaveChanges();

            return s;
        }

        public StudentModule UpdateStudentModuleGrade(int studentId, int moduleId, double grade)
        {
            var sm = db.StudentModules.FirstOrDefault(o => o.StudentId == studentId && o.ModuleId == moduleId);
            if (sm == null)
            {
                return null; // no such student module
            }
            sm.Grade = grade;
            db.SaveChanges();

            // could possibly Recalculate the overall grade here            
            //RecalculateStudentGrade(studentId);

            return sm;
        }

        public Student RecalculateStudentGrade(int studentId)
        {
            // retrieve the student identified by studentId
            var s = GetStudent(studentId);
            // check the student exists and if not return null
            if (s == null)
            {
                return null;
            }
            // calculate average of module grades 
            var sum = s.StudentModules.Sum(sm => sm.Grade);
            var count = s.StudentModules.Count();
            var avg = sum / (count == 0 ? 1 : count);
            //var avg = s.StudentModules.DefaultIfEmpty().Average(sm => sm.Grade);            
            // set the student Profile Grade with average of module grades or 0 if no modules
            s.Profile.Grade = avg;            
            // save the changes
            db.SaveChanges();
            // return the updated student
            return s;
        }

        public StudentModule GetStudentModule(int studentId, int moduleId)
        {
            return db.StudentModules.FirstOrDefault(sm => sm.StudentId == studentId && sm.ModuleId == moduleId);
        }

        public StudentModule AddStudentToModule(int studentId, int moduleId)
        {
            // check that this student module does not already exist
            var exists = db.StudentModules.FirstOrDefault(o => o.StudentId == studentId &&
                                                           o.ModuleId == moduleId);
            if (exists != null)
            {
                return null; // already exists so no new student module added
            }

            // now verify that both the student and module exist
            var s = GetStudent(studentId);
            var m = GetModule(moduleId);
            if (s == null || m == null)
            {
                return null; // at least one of them doesnt exist so cant create new student module
            }

            // now create and add the new student module
            var sm = new StudentModule { Student = s, Module = m, Grade = 0.0 };
            // alternative way to create the student module - just set the foreign keys
            // var sm = new StudentModule {StudentId = studentId, ModuleId = moduleId, Grade = 0.0 };
            db.StudentModules.Add(sm);

            db.SaveChanges();
            return sm;
        }
        public bool RemoveStudentFromModule(int studentId, int moduleId)
        {
            // try to locate the student module
            var sm = db.StudentModules.FirstOrDefault(o => o.StudentId == studentId &&
                                                           o.ModuleId == moduleId);
            if (sm == null)
            {
                return false; //not found
            }

            // remove the student module
            db.StudentModules.Remove(sm);
            db.SaveChanges();
            return true;
        }

        public IList<Student> GetStudents()
        {
            return db.Students.Include(s => s.Profile).ToList();
        }

        public IList<Student> GetStudentsTakingModule(int moduleId)
        {
            // here we ensure we return students with their related profile
            // this ensures consistency with the GetStudent method that also
            // returns the student Profile
            return db.StudentModules
                        .Where(sm => sm.ModuleId == moduleId)
                        .Include(sm => sm.Student.Profile)
                        .Select(sm => sm.Student).ToList();
        }

        public IList<Ticket> GetActiveTickets()
        {
            // return tickets with related student and student profile
            return db.Tickets
                    .Where(t => t.Active)
                    .Include(t => t.Student)
                    .Include(t => t.Student.Profile).ToList();
        }

        public IList<Ticket> GetStudentTickets(int studentId)
        {
            // return ALL tickets for specified student
            return db.Tickets
                    .Where(t => t.StudentId == studentId)
                    .Include(t => t.Student)
                    .Include(t => t.Student.Profile).ToList();
        }

        public Ticket GetTicket(int id)
        {
            return db.Tickets.FirstOrDefault(t => t.Id == id);
        }

        public Ticket CreateTicket(int studentId, string issue)
        {
            // verify student
            var s = GetStudent(studentId);
            if (s == null)
            {
                return null; // no such student so cant create ticket
            }

            // create ticket
            var t = new Ticket
            {
                Issue = issue,
                CreatedOn = DateTime.Now,
                Active = true,
                StudentId = studentId
            };

            // add ticket
            db.Tickets.Add(t);
            db.SaveChanges();
            return t;
        }

        public Ticket CloseTicket(int id, string response=null)
        {
            var ticket = db.Tickets.FirstOrDefault(t => t.Id == id && t.Active);
            if (ticket == null)
            {
                return null; // no such ticket so cant close
            }
            // update the ticket as closed
            ticket.Active = false;
            ticket.Response = response;
            db.SaveChanges();
            return ticket;
        }

        public int ActiveTicketCount()
        {
            return db.Tickets.Count(t => t.Active);
        }

        public int ActiveTicketCountForStudent(int id)
        {
            return db.Tickets.Count(t => t.Active && t.Student.Id == id);
        }

        public int StudentCount()
        {
            return db.Students.Count();
        }

        public User GetUserByName(string username)
        {
            return db.Users.FirstOrDefault(u => u.Username == username);
        }

        // Authentication
        public User GetUserByCredentials(string username, string password)
        {
            return db.Users.FirstOrDefault(s => s.Username == username && s.Password == password);
        }

        // Register new user
        public User RegisterUser(string username, string password, Role role)
        {
            var o = GetUserByCredentials(username, password);
            if (o != null)
            {
                return null;
            }
            // user is unique so store in database
            var user = new User { Username = username, Password = password, Role = role };
            
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }
    }
}