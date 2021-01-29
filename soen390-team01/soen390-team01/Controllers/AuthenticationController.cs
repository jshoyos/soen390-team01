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
        [BindProperty]
        public LoginModel Input { get; set; }
        private AuthenticationFirebaseService _authService = new AuthenticationFirebaseService();
        private IDataProtector _provider;
        
        public AuthenticationController(IDataProtectionProvider provider)
        {
            _provider = provider.CreateProtector("asp.AuthenticationController");
        }
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
                    return View(model);
                }
                setAuthCookie(model.Email, this.HttpContext);
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
    }
}
