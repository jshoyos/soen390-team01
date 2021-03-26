using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    public class AuthenticationController : Controller
    {
        #region fields
        private readonly AuthenticationFirebaseService _authService;
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<AuthenticationController> _log;
        #endregion

        public AuthenticationController(AuthenticationFirebaseService authService,
            IUserManagementService userManagementService,
            ILogger<AuthenticationController> log)
        {
            _authService = authService;
            _userManagementService = userManagementService;
            _log = log;
        }

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
        public IActionResult IndexAsync(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var email = model.Email;
                    var password = model.Password;
                    var user = AuthenticateUser(email, password);
                    _log.LogInformation($"{email} trying to log in");
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid authentication");
                        _log.LogWarning($"{email} failed authentication");
                        return View(model);
                    }
                    _authService.SetAuthCookie(email, user.Role, this.HttpContext);
                    _log.LogInformation($"{email} authentication successful");
                    return LocalRedirect("/Home/Privacy");
                }
                catch (DataAccessException)
                {
                    ModelState.AddModelError(string.Empty, "Invalid authentication");
                    return View(model);
                }
            }

            return View(model);
        }

        public IActionResult LogoutAsync()
        {
            _authService.RemoveAuthCookie(this.HttpContext);
            _log.LogInformation("User logged out");
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
        public IActionResult ForgotPassword(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                return View(model);
            }

            _authService.RequestPasswordChange(model.Email);
            return LocalRedirect("/Authentication/Index");

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
            var user = _userManagementService.GetUserByEmail(email);
            if (user != null && _authService.AuthenticateUser(email, password))
            {
                return user;
            }
            return null;
        }

        #endregion
    }
}
