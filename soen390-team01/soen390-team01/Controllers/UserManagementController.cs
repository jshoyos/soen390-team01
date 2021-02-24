using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
using soen390_team01.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace soen390_team01.Controllers
{
    public class UserManagementController : Controller
    {
        #region fields
        private readonly AuthenticationFirebaseService _authService;
        private readonly UserManagementService _userManagementService;
        #endregion

        public UserManagementController(AuthenticationFirebaseService authService, UserManagementService userManagementService)
        {
            _authService = authService;
            _userManagementService = userManagementService;
        }

        #region properties
        [BindProperty]
        public AddUserModel AddUserInput { get; set; }
        [TempData]
        public string StringErrorMessage { get; set; }
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
        /// Event Handler when the there is a request to add a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UserManagement(UserManagementModel model)
        {
            if (!ModelState.IsValid || !RegisterUser(model.AddUser))
            {
                ModelState.AddModelError("Result", "Couldn't add user");
                model.Users = _userManagementService.GetAllUsers();
                return View(model);
            }
            return RedirectToAction("UserManagement");
        }
        public IActionResult GetUserById(long userId)
        {
            var user = _userManagementService.GetUserById(userId);

            if (user != null)
            {
                return PartialView("_UserModalPartial", user);
            }
            return UserManagement();
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            ModelState["Password"].Errors.Clear();
            ModelState["Password"].ValidationState = ModelValidationState.Valid;
            ModelState["ConfirmPassword"].Errors.Clear();
            ModelState["ConfirmPassword"].ValidationState = ModelValidationState.Valid;
            ModelState["Email"].Errors.Clear();
            ModelState["Email"].ValidationState = ModelValidationState.Valid;
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
            if (addedUser != null && _authService.RegisterUser(addedUser.Email, user.Password).Result)
            {
                _userManagementService.RemoveUser(user);
                return false;
            }
            return true;
        }

        #endregion
    }

}
