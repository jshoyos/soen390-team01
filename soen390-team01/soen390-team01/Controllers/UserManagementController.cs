using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UserManagementService _userManagementService;
        #endregion

        public UserManagementController(AuthenticationFirebaseService authService,
            UserManagementService userManagementService)
        {
            _authService = authService;
            _userManagementService = userManagementService;
        }


        #region Methods

        [HttpGet]
        [ModulePermissionAttribute(Roles = Role.Admin)]
        public IActionResult Index()
        {
            var model = new UserManagementModel
            {
                Users = _userManagementService.GetAllUsers(),
                AddUser = new AddUserModel(),
                EditUser = new EditUserModel()
            };

            return View("Index", model);
        }

        /// <summary>
        ///     Event Handler when the there is a request to add a new user
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
                    RegisterUser(model.AddUser);
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
                var user = _userManagementService.GetUserById(userId);

                if (user != null)
                {
                    return PartialView("_UserModalPartial", new EditUserModel(user));
                }
                return Index();

            }
            catch (NotFoundException e)
            {
                TempData["errorMessage"] = e.Message;
                return Index();
            }
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

        private void RegisterUser(AddUserModel user)
        {
            var addedUser = _userManagementService.AddUser(user);
            if (_authService.RegisterUser(addedUser.Email, user.Password).Result)
            {
                return;
            }

            _userManagementService.RemoveUser(user);
            throw new AccountRegistrationException();
        }

        #endregion
    }
}