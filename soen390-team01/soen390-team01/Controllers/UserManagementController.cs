using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        #region fields
        private readonly AuthenticationFirebaseService _authService;
        private readonly IUserManagementService _model;
        private readonly ILogger<UserManagementController> _log;
        #endregion

        public UserManagementController(AuthenticationFirebaseService authService,
            IUserManagementService model,
            ILogger<UserManagementController> log)
        {
            _authService = authService;
            _model = model;
            _log = log;
        }

        #region Methods

        [HttpGet]
        [ModulePermission(Roles = Role.Admin)]
        public IActionResult Index()
        {
            _model.Reset();

            return View("Index", _model);
        }

        /// <summary>
        /// Event Handler when the there is a request to add a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddUser(UserManagementModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    RegisterUser(model.AddUserModel);
                    _log.LogInformation($"Adding user {model.AddUserModel.Email} with role: {model.AddUserModel.Role}");
                }
                catch (DataAccessException e)
                {
                    TempData["errorMessage"] = e.ToString();
                }
            }

            return Index();
        }

        public IActionResult GetUserById(long userId)
        {
            try
            {
                var user = _model.GetUserById(userId);

                if (user != null)
                {
                    return PartialView("_UserModalPartial", new EditUserModel(user));
                }
            }
            catch (NotFoundException e)
            {
                TempData["errorMessage"] = e.Message;
            }
            return Index();
        }

        [HttpPost]
        public IActionResult EditUser(EditUserModel user)
        {
            if (ModelState.IsValid)
            {
                var editedUser = _model.EditUser(user);
                _log.LogInformation($"Modifying user {user.Email}");
                return PartialView("_UserRowPartial", editedUser);
            }

            return PartialView("_UserModalPartial", user);
        }

        private void RegisterUser(AddUserModel user)
        {
            var addedUser = _model.AddUser(user);
            if (_authService.RegisterUser(addedUser.Email, user.Password))
            {
                return;
            }

            _model.RemoveUser(user);
            throw new AccountRegistrationException();
        }

        #endregion
    }
}