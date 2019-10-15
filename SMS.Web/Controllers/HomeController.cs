using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMS.Web.Models;
using SMS.Core.Models;

namespace SMS.Web.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            // demonstrating use of viewbag to send data to a view   
            ViewBag.Date = DateTime.Now;
            ViewBag.Message = "You can login or register for a new account via the Login option on menu bar.";
            return View();
        }

        public IActionResult About()
        {   
            // demonstrating use of a ViewModel to send data to a view    
            var about = new AboutViewModel();
            about.Formed = new DateTime(2019, 01, 01);
            about.Days = (DateTime.Now - about.Formed).Days;
            about.Message = "The Student Management System(SMS) is a web development company. We were formed as part of the delivery of MSc Professional Software Development (COM741)";

            return View(about);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
