using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
using soen390_team01.Services;
using Microsoft.AspNetCore.Authorization;

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
        public IActionResult Index()
        {
            ModelState.Clear();

            var model = new UserManagementModel
            {
                Users = _userManagementService.GetAllUsers(),
                AddUser = new AddUserModel()
            };

            return View("Index", model);
        }
        /// <summary>
        /// Event Handler when the there is a request to add a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddUser(UserManagementModel model)
        {
            if (!RegisterUser(model.AddUser))
            {
                ModelState.AddModelError(string.Empty, "Couldn't add user");
            }
            return Index();
        }
        public IActionResult GetUserById(long userId)
        {
            var user = _userManagementService.GetUserById(userId);

            if (user != null)
            {
                return PartialView("_UserModalPartial", user);
            }
            return Index();
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            var editedUser = _userManagementService.EditUser(user);

            return PartialView("_UserRowPartial", editedUser);
        }

        private bool RegisterUser(AddUserModel user)
        {
            // Decrypted added user
            var addedUser = _userManagementService.AddUser(user);
            return addedUser != null && _authService.RegisterUser(addedUser.Email, user.Password).Result;
        }

        #endregion
    }

}
