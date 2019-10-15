using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Core.Models;

namespace SMS.Web.Models
{
    public class ModuleViewModel
    {
        // selectlist of modules (id, title)       
        public SelectList Modules { get; set; }

        // Collecting ModuleId and Grade in Form
        [Required]
        [Display(Name = "Module")]
        [Remote(action: "IsUniqueModule", controller: "Student", AdditionalFields = nameof(StudentId))]
        public int ModuleId { get; set; }

        public int StudentId { get; set; }

        public double Grade { get; set; }

    }
}