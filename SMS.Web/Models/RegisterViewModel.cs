using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SMS.Core.Models;

namespace SMS.Web.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Remote(action: "IsUniqueUsername", controller: "User")]
        public string Username { get; set; }
 
        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string PasswordConfirm  { get; set; }

        [Required]
        public Role Role { get; set; }

    }
}