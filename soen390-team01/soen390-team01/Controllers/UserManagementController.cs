using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01.Controllers
{
    public class UserManagementController : Controller
    {
        public UserManagementController(AuthenticationFirebaseService authService,
            UserManagementService userManagementService)
        {
            _authService = authService;
            _userManagementService = userManagementService;
        }

        #region fields

        private readonly AuthenticationFirebaseService _authService;
        private readonly UserManagementService _userManagementService;

        #endregion

        #region properties

        #endregion

        #region Methods

        [HttpGet]
        [Authorize]
        public IActionResult UserManagement()
        {
            var model = new UserManagementModel
            {
                Users = _userManagementService.GetAllUsers(),
                AddUser = new AddUserModel()
            };

            return View(model);
        }

        /// <summary>
        ///     Event Handler when the there is a request to add a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UserManagement(UserManagementModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    RegisterUser(model.AddUser);
                }
                catch (DbContextException e)
                {
                    TempData["errorMessage"] = e.ToString();
                }
            }

            return UserManagement();
        }

        public IActionResult GetUserById(long userId)
        {
            var user = _userManagementService.GetUserById(userId);

            if (user != null)
            {
                return PartialView("_UserModalPartial", new EditUserModel(user));
            }

            return UserManagement();
        }

        [HttpPost]
        public IActionResult EditUser(EditUserModel user)
        {
            if (ModelState.IsValid)
            {
                var editedUser = _userManagementService.EditUser(user);
                return PartialView("_UserRowPartial", editedUser);
            }

            return PartialView("_UserModalPartial", user);
        }

        private bool RegisterUser(AddUserModel user)
        {
            // Decrypted added user

            var addedUser = _userManagementService.AddUser(user);
            if (addedUser == null || !_authService.RegisterUser(addedUser.Email, user.Password).Result)
            {
                if (addedUser != null)
                {
                    _userManagementService.RemoveUser(user);
                }

                return false;
            }

            return true;
        }

        #endregion
    }
}