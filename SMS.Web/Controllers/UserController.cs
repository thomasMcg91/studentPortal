using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using SMS.Core.Models;
using SMS.Data.Services;
using SMS.Web.Models;

namespace SMS.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IStudentService _svc;

        public UserController()
        {
            _svc = new StudentService();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")]LoginViewModel m)
        {        
            // call service to locate user
            var user = _svc.GetUserByCredentials(m.Username, m.Password);
            if (user == null)
            {
                ModelState.AddModelError("Username", "Invalid Login Credentials");
                ModelState.AddModelError("Password", "Invalid Login Credentials");
                return View(m);
            }
           
            // sign user in using cookie authentication to store principal
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                BuildClaimsPrincipal(user)
            );

            return RedirectToAction("Index","Student");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Username,Password,PasswordConfirm,Role")]RegisterViewModel m)
        {
            if (ModelState.IsValid)
            {
                // register the user
                var user = _svc.RegisterUser(m.Username, m.Password, m.Role);

                // sign user in using cookie authentication to store principal
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    BuildClaimsPrincipal(user)
                );

                // redirect to home page
                return RedirectToAction("Index", "Home");
            }
            return View(m);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // delete the login cookie and redirect to login page
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult ErrorNotAuthorised()
        {   
            // called when user navigates to a route that requires authorisation
            Alert("Not Authorized", AlertType.warning);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErrorNotAuthenticated()
        {
            // called when user navigates to route that requires authentication
            Alert("Not Authenticated", AlertType.warning);
            return RedirectToAction("Login", "User"); 
        }

        // Build a claims principal from authenticated user
        private  ClaimsPrincipal BuildClaimsPrincipal(User user)
        { 
            // define user claims including a custom claim for user Id
            // this would be useful if any future queries/actions required
            // user Id to be submitted with requests
            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            // build principal using claims
            return  new ClaimsPrincipal(claims);
        }


        // Validation method used to verify uniqueness of username when registering
        [AcceptVerbs("Get","Post")]
        public IActionResult IsUniqueUsername(string Username)
        {
            var u = _svc.GetUserByName(Username);
            return (u == null) ? Json( data: true) : Json(data: $"The Username {Username} already exists.");
        }

    }
}
