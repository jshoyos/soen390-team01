using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Models;
using soen390_team01.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Controllers
{
    public class RegisterController : Controller
    {
        #region fields
        private string _email;
        private string _protectedPassword;
        private AuthenticationFirebaseService _authService = new AuthenticationFirebaseService();
        private IDataProtector _provider;
        #endregion

        #region properties
        [BindProperty]
        public RegisterModel registerInput { get; set; }
        [TempData]
        public string StringErrorMessage { get; set; }
        #endregion

        public RegisterController(IDataProtectionProvider provider)
        {
            _provider = provider.CreateProtector("asp.RegisterController");
        }

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OnPost(RegisterModel model)
        {
            if (validateInput(model.Email, model.Password, model.ConfirmPassword))
            {
                _email = model.Email;
                _protectedPassword = _provider.Protect(model.Password);
            }

            if (ModelState.IsValid)
            {
                if (AddUser(_email, _protectedPassword))
                {
                    return LocalRedirect("/Home/Privacy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The account failed to be created");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error");
                return View();
            }
        }
        private bool validateInput(string email, string password, string confirmPassowrd)
        {
            if (!(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassowrd)))
            {
                if (password.Equals(confirmPassowrd))
                {
                    return true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Passwords do not match");
                    return false;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Password Field cannot be empty");
                return false;
            }
        }
        private bool AddUser(string email, string password)
        {
            if (_authService.RegisterUser(email, password).Result)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
