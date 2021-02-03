using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Models;
using soen390_team01.Services;
using System.Collections.Generic;
using System.Security.Claims;

namespace soen390_team01.Controllers
{
    public class AuthenticationController : Controller
    {
        #region fields
        private string _email;
        private string _password;
        private AuthenticationFirebaseService _authService;
        #endregion

        public AuthenticationController(AuthenticationFirebaseService authService) 
        {
            _authService = authService;
        }
        #region properties
        [BindProperty]
        public LoginModel Input { get; set; }
        [TempData]
        public string StringErrorMessage { get; set; }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }
        /// <summary>
        /// Event handler when the user sends a password reset request
        /// </summary>
        /// <param name="model"></param>
        public void SendPasswordRequest(LoginModel model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                _authService.RequestPasswordChange(model.Email);
            }
            ModelState.AddModelError(string.Empty, "Email cannot be empty");
        }
        /// <summary>
        /// Event handler when the user presses the button to login.
        /// Returns to the login page if there is an error with the Authentication otherwise it redirects to the home page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult OnPost(LoginModel model)
        {
            if (ValidateInput(model) && ModelState.IsValid)
            {
                _email = model.Email;
                _password = model.Password;
                var user = AuthenticateUser(_email, _password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid authentication");
                    return View("Index");
                }
                SetAuthCookie(_email, this.HttpContext);
                return LocalRedirect("/Home/Privacy");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error");
                return View("Index");
            }
        }
        /// <summary>
        /// Validates both the email and password fields
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <returns>true if inputs pass the validation false otherwise</returns>
        private bool ValidateInput(LoginModel model)
        {
            if (!(string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password)))
            {
                return true;
            }
            else
            {
                ModelState.AddModelError(string.Empty,string.IsNullOrEmpty(model.Email) 
                    ? "Email field cannot be empty "
                    : "Password Field cannot be empty");
                return false;
            }
        }
        /// <summary>
        /// Authenticates the user with the help of the firebase services
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <returns>returns the current user</returns>
        private string AuthenticateUser(string email, string password)
        {
            if (_authService.AuthenticateUser(email, password).Result)
            {
                //TODO: return more than just a string
                return "User";
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the authentication cookie so user is remembered in the browser
        /// </summary>
        /// <param name="email"></param>
        /// <param name="context"></param>
        private async void SetAuthCookie(string email, HttpContext context)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
            };

            await AuthenticationHttpContextExtensions.SignInAsync(context, CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }
        // this method might go in another controller later on
        //private async void removeAuthCookie(HttpContext context)
        //{
        //    await AuthenticationHttpContextExtensions.SignOutAsync(context, CookieAuthenticationDefaults.AuthenticationScheme);
        //}
        #endregion
    }
}
