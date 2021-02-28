using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Models;
using soen390_team01.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace soen390_team01.Controllers
{
    public class AuthenticationController : Controller
    {
        #region fields

        private readonly AuthenticationFirebaseService _authService;
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
        /// <summary>
        /// Get for the login
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Post action for the login submit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> IndexAsync(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string email = model.Email;
                string password = model.Password;
                var user = AuthenticateUser(email, password);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid authentication");
                    return View(model);
                }
                await SetAuthCookie(email, this.HttpContext);
                return LocalRedirect("/Home/Privacy");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error");
                return View(model);
            }
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await RemoveAuthCookie(this.HttpContext);
            return LocalRedirect("/Authentication/Index");
        }
        /// <summary>
        /// Get for the Forgot Password
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Post action for the forgot password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(LoginModel model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                await _authService.RequestPasswordChange(model.Email);
                return LocalRedirect("/Authentication/Index");
            }
            else
            {
                //ModelState.AddModelError(string.Empty, "Email cannot be empty");
                return View(model);
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
        private static async Task SetAuthCookie(string email, HttpContext context)
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

        private static async Task RemoveAuthCookie(HttpContext context)
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(context, CookieAuthenticationDefaults.AuthenticationScheme);
        }
        #endregion
    }
}
