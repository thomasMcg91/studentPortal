using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Web.Models
{
    public class TicketViewModel
    {
        public int Id { get; set; }

        // selectlist of students (id, name)       
        public SelectList Students { set; get; }

        // Collecting StudentId and Issue in Form
        [Required]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Issue { get; set; }
               
        [StringLength(500)]
        public string Response { get; set; }

    }

}
