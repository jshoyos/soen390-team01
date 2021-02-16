using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
using soen390_team01.Services;
using System.Collections.Generic;

namespace soen390_team01.Controllers
{
    public class RegisterController : Controller
    {
        #region fields
        private readonly AuthenticationFirebaseService _authService;
        private readonly UserManagementService _userManagementService;
        #endregion

        public RegisterController(AuthenticationFirebaseService authService, UserManagementService userManagementService)
        {
            _authService = authService;
            _userManagementService = userManagementService;
        }

        #region properties
        [BindProperty]
        public RegisterModel RegisterInput { get; set; }
        [TempData]
        public string StringErrorMessage { get; set; }
        #endregion

        #region Methods
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Users"] = _userManagementService.GetAllUsers();
            return View();
        }
        /// <summary>
        /// Event Handler when the there is a request to add a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (AddUser(model))
                {
                    return LocalRedirect("/Home/Privacy");
                }
            }
            return View();
        }

        public IActionResult GetUserById(long id)
        {
            var user = _userManagementService.GetUserById(id);

            if (user != null)
            {
                RegisterModel model = new RegisterModel();
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Role = user.Role;
                model.PhoneNumber = user.PhoneNumber;
                model.Email = user.Email;

                return PartialView("_UserModalPartial", model);
            }
            return View("Index");
        }

        /// <summary>
        /// Add the user to the database and to the firebase authentication service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool AddUser(RegisterModel model)
        {
            return _userManagementService.AddUser(model) && _authService.RegisterUser(model.Email, model.Password).Result;
        }
        #endregion
    }
}
