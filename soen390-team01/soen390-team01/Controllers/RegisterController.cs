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
        private string _password;
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
        /// <summary>
        /// Event Handler when the there is a request to add a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult OnPost(RegisterModel model)
        {
            if (validateInput(model.Email, model.Password, model.ConfirmPassword))
            {
                _email = model.Email;
                _password = model.Password;
            }

            if (ModelState.IsValid)
            {
                if (AddUser(_email, _password))
                {
                    return LocalRedirect("/Home/Privacy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The account failed to be created");
                    return View("Index");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error");
                return View();
            }
        }
        /// <summary>
        /// Validates the inputs to Register a new user
        /// Checks that all the fields are completed
        /// Checks that both the password and the confirmation of the password match
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <param name="confirmPassowrd">confirmation of the password</param>
        /// <returns></returns>
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
        /// <summary>
        /// Uses the firebase service to add the user with email and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
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
