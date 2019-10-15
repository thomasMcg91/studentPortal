using SMS.Web.Models;
using SMS.Core.Models;
using SMS.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace SMS.Web.Controllers
{
    [Authorize]
    public class StudentController : BaseController
    {
        private readonly StudentService svc;
        public StudentController()
        {
            svc = new StudentService();
        }

        // GET /student/index
        public IActionResult Index()
        {
            var students = svc.GetStudents();
            return View(students);
        }

        // GET /student/details/{id}
        public IActionResult Details(int id)
        {
            var student = svc.GetStudent(id);
            if (student == null)
            {
                Alert("Student Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            return View(StudentViewModel.FromStudent(student));
        }

        [Authorize(Roles = "Admin")]
        // GET /student/create
        public IActionResult Create()
        {
            // render blank form
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST /student/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Course,Age,Email")]StudentViewModel vm)
        {
            if (ModelState.IsValid)
            {
                //  add student via service
                var s = svc.AddStudent(vm.ToStudent());
                Alert("Student created successfully",AlertType.success);
                return RedirectToAction(nameof(Details), new { Id = s.Id });
            }
            // redisplay the form for editing
            return View(vm);
        }

        [Authorize(Roles = "Admin,Manager")]
        // GET /student/edit/{id}
        public IActionResult Edit(int id)
        {
            // load student via service
            var s = svc.GetStudent(id);
            if (s == null)
            {
                Alert("Student Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            // pass student to view for editing
            return View( StudentViewModel.FromStudent(s) );
        }

        [Authorize(Roles = "Admin,Manager")]
        // POST /student/edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id, Name, Course, Age, Email")] StudentViewModel s)
        {
            if (ModelState.IsValid)
            {
                svc.UpdateStudent(s.ToStudent());
                Alert("Student updated successfully", AlertType.success);
                return RedirectToAction(nameof(Details), new { Id = s.Id });
            }
            // redisplay the form for editing
            return View(s);
        }

        [Authorize(Roles = "Admin")]
        // GET / student/delete/{id}
        public IActionResult Delete(int id)
        {
            // load student via service 
            var s = svc.GetStudent(id);
            
            // pass student to view for deletion confirmation
            return View( s );
        }

        [Authorize(Roles = "Admin")]
        // POST /student/delete/{id}
        [HttpPost]
        public IActionResult DeleteConfirm(int id)
        {
            // delete student via service
            svc.DeleteStudent(id);

            Alert("Student deleted successfully", AlertType.success);
            // redirect to the index view
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin,Manager")]
        // GET /student/createticket/{id}
        public IActionResult CreateTicket(int id)
        {
            var s = svc.GetStudent(id);
            if (s == null)
            {
                Alert("Student Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            // create a ticket view model and set foreign key
            var tvm = new TicketViewModel { StudentId = id }; 
            // render blank form
            return View( tvm );
        }

        [Authorize(Roles = "Admin,Manager")]
        // POST /student/create
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult CreateTicket([Bind("StudentId,Issue")]TicketViewModel t)
        {         
            if (ModelState.IsValid)
            {                
                svc.CreateTicket(t.StudentId, t.Issue);

                return RedirectToAction(nameof(Details), new { Id = t.StudentId });
            }
            // redisplay the form for editing
            return View(t);
        }

        [Authorize(Roles = "Admin,Manager")]
        // GET /student/addmodule/{id}
        public IActionResult AddModule(int id)
        {
            var s = svc.GetStudent(id);
            if (s == null)
            {
                Alert("Student Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }

            var modules = svc.GetModules();
            // create a ticket view model and set foreign key
            var tvm = new ModuleViewModel {
                StudentId = id,
                Modules = new SelectList(modules, "Id", "Title")
            };
            // render blank form
            return View(tvm);
        }

        [Authorize(Roles = "Admin,Manager")]
        // POST /student/addmodule
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult AddModule([Bind("StudentId,ModuleId,Grade")]ModuleViewModel m)
        {
            if (ModelState.IsValid)
            { 
                var sm = svc.AddStudentToModule(m.StudentId, m.ModuleId);
                svc.UpdateStudentModuleGrade(m.StudentId, m.ModuleId, m.Grade);
                svc.RecalculateStudentGrade(m.StudentId);
                //if (sm != null)
                //{
                //    svc.UpdateStudentModuleGrade(m.StudentId, m.ModuleId, m.Grade);
                //    svc.RecalculateStudentGrade(m.StudentId);
                //}
                //else
                //{
                //    ModelState.AddModelError("ModuleId", "Module is already taken by Student");
                //    m.Modules = new SelectList(svc.GetModules(),"Id","Title");
                //    return View(m);
                //}
                return RedirectToAction(nameof(Details), new { Id = m.StudentId });
            }

            m.Modules = new SelectList(svc.GetModules(), "Id", "Title");

            // redisplay the form for editing
            return View(m);
        }

        [Authorize(Roles = "Admin,Manager")]
        // GET /student/editmodule/
        public IActionResult EditModule(int studentId, int moduleId)
        {
            var s = svc.GetStudent(studentId);
            var m = s.StudentModules.FirstOrDefault(sm => sm.ModuleId == moduleId);
            if (s == null)
            {
                Alert("Student Not Found", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }


            // create a ticket view model and set attributes
            var tvm = new ModuleViewModel
            {
                StudentId = studentId,
                ModuleId = moduleId,
                Grade = m.Grade
            };
            // render blank form
            return View(tvm);
        }

        [Authorize(Roles = "Admin,Manager")]
        // POST /student/addmodule
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult EditModule([Bind("StudentId,ModuleId,Grade")]ModuleViewModel m)
        {
            if (ModelState.IsValid)
            {
                svc.UpdateStudentModuleGrade(m.StudentId, m.ModuleId, m.Grade);
                svc.RecalculateStudentGrade(m.StudentId);            
                
                return RedirectToAction(nameof(Details), new { Id = m.StudentId });
            }

            // redisplay the form for editing
            return View(m);
        }


        [Authorize(Roles = "Admin,Manager")]
        //  POST /student/removemodule/{id}
        [HttpPost]
        public IActionResult RemoveModule(int studentId, int moduleId)
        {
            // close ticket via service
            if (!svc.RemoveStudentFromModule(studentId, moduleId))
            {
                Alert("Problem removing module from student", AlertType.warning);
                return RedirectToAction(nameof(Details), new { Id = studentId });
            }
            svc.RecalculateStudentGrade(studentId);

            // redirect to the index view
            return RedirectToAction(nameof(Details), new { Id = studentId });
        }

        public IActionResult IsUniqueModule(int moduleId, int studentId)
        {
            var m = svc.GetStudentModule(studentId, moduleId);
            return (m == null) ? Json(data: true) : Json(data: $"The module is already taken by the student");
        }

    }
}
