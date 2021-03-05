using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;
using soen390_team01.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace soen390_team01.Controllers
{
    public class AuthenticationController : Controller
    {
        #region fields

        private readonly AuthenticationFirebaseService _authService;
        private readonly UserManagementService _userManagementService;
        #endregion

        public AuthenticationController(AuthenticationFirebaseService authService,
            UserManagementService userManagementService)
        {
            _authService = authService;
            _userManagementService = userManagementService;
        }
        #region properties
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
                try
                {
                    var email = model.Email;
                    var password = model.Password;
                    var user = AuthenticateUser(email, password);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid authentication");
                        return View(model);
                    }
                    await _authService.SetAuthCookie(email, user.Role, this.HttpContext);
                    return LocalRedirect("/Home/Privacy");
                }
                catch (NotFoundException)
                {
                    TempData["errorMessage"] = "User does not exist";
                }
                catch(EmailNotFoundException)
                {
                    TempData["errorMessage"] = "User does not exist";
                }
            }

            return View(model);
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await _authService.RemoveAuthCookie(this.HttpContext);
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

            return View(model);
        }

        [HttpGet]
        public IActionResult PermissionDenied()
        {
            return View();
        }

        /// <summary>
        /// Authenticates the user with the help of the firebase services
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <returns>returns the current user</returns>
        private User AuthenticateUser(string email, string password)
        {
            User user = _userManagementService.GetUserByEmail(email);
            if (user != null && _authService.AuthenticateUser(email, password).Result)
            {
                return user;
            }
            return null;
        }

        #endregion
    }
}
