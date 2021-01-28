using Microsoft.AspNetCore.Mvc;
using soen390_team01.Models;
using soen390_team01.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Controllers
{
    public class AuthenticationController : Controller
    {
        [BindProperty]
        public LoginModel Input { get; set; }
        private AuthenticationService _authService = new AuthenticationService();

        public IActionResult Index()
        {
            ViewBag.Model = Input;
            return View();
        }
        [TempData]
        public string StringErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = AuthenticateUser(ViewData["Email"].ToString(), ViewData["Password"].ToString());
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid authentication");
                    return View();
                }
                return LocalRedirect("/Home/Privacy");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error");
                return View();
            }
        }

        private string AuthenticateUser(string email, string password)
        {
            if (_authService.AuthenticateUser(email, password).Result)
            {
                return "User";
            }
            else
            {
                return null;
            }
        }
    }
}
