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
        private string _protectedPassword;
        private AuthenticationFirebaseService _authService = new AuthenticationFirebaseService();
        private IDataProtector _provider;
        #endregion

        public AuthenticationController(IDataProtectionProvider provider)
        {
            _provider = provider.CreateProtector("asp.AuthenticationController");
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

        public void SendPasswordRequest(LoginModel model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                _authService.RequestPasswordChange(model.Email);
            }
            ModelState.AddModelError(string.Empty, "Email cannot be empty");
        }
        public IActionResult OnPost(LoginModel model)
        {
            if (validateInput(model.Email, model.Password))
            _email = model.Email;
            _protectedPassword = _provider.Protect(model.Password);

            if (ModelState.IsValid)
            {
                var user = AuthenticateUser(_email, _protectedPassword);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid authentication");
                    return LocalRedirect("/");
                }
                setAuthCookie(_email, this.HttpContext);
                return LocalRedirect("/Home/Privacy");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error");
                return LocalRedirect("/");
            }
        }

        private bool validateInput(string email, string password)
        {
            if (!(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)))
            {
                return true;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Password Field cannot be empty");
                return false;
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


        private async void setAuthCookie(string email, HttpContext context)
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

        private async void removeAuthCookie(HttpContext context)
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(context, CookieAuthenticationDefaults.AuthenticationScheme);
        }
        #endregion
    }
}
