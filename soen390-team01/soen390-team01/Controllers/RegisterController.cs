using Microsoft.AspNetCore.Mvc;
using soen390_team01.Models;
using soen390_team01.Services;

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

        /// <summary>
        /// Uses the firebase service to add the user with email and password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool AddUser(RegisterModel model)
        {
            if(_authService.RegisterUser(model.Email, model.Password).Result)
            { 
                _userManagementService.AddUser(model);
            } 
            return true;
        }

        private void FetchDate()
        {

        }
        #endregion
    }
}
