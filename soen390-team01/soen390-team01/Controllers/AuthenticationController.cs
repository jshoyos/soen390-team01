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
            return View();
        }
        [TempData]
        public string StringErrorMessage { get; set; }

        public IActionResult OnPost(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = AuthenticateUser(model.Email, model.Password);
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
