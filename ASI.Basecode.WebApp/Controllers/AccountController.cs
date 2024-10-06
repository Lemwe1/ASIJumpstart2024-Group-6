using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly IUserService _userService;

        public AccountController(
                             SignInManager signInManager,
                             IHttpContextAccessor httpContextAccessor,
                             ILoggerFactory loggerFactory,
                             IConfiguration configuration,
                             IUserService userService) : base(httpContextAccessor, loggerFactory, configuration)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._userService = userService;
        }

        /// <summary>
        /// Login Method
        /// </summary>
        /// <returns>Created response view</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            TempData["returnUrl"] = System.Net.WebUtility.UrlDecode(HttpContext.Request.Query["ReturnUrl"]);
            this._sessionManager.Clear();
            this._session.SetString("SessionId", System.Guid.NewGuid().ToString());
            return this.View(new LoginViewModel());  // Pass LoginViewModel to view
        }

        /// <summary>
        /// Authenticate user and signs the user in when successful.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns> Created response view </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide valid credentials.";
                return View(model);  // Return with the same model in case of errors
            }

            this._session.SetString("HasSession", "Exist");

            MUser user = null;
            var loginResult = _userService.AuthenticateUser(model.UserCode, model.Password, ref user);
            if (loginResult == LoginResult.Success)
            {
                // Sign in the user
                await this._signInManager.SignInAsync(user);
                this._session.SetString("UserName", string.Join(" ", user.FirstName, user.LastName));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Incorrect UserId or Password";
                return View(model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());  // Pass RegisterViewModel to view
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return View(model);  // Return with the same model in case of errors
            }

            if (model.Password != model.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match.";
                return View(model);
            }

            // Check if the user code is already taken
            if (_userService.RetrieveAll(null, model.UserCode).Any())
            {
                TempData["ErrorMessage"] = "UserCode is already taken.";
                return View(model);
            }

            try
            {
                var newUser = new UserViewModel
                {
                    UserCode = model.UserCode,
                    Password = model.Password, // Password will be encrypted in UserService.Add()
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Mail = model.Email
                };

                // Add the new user
                _userService.Add(newUser);

                TempData["SuccessMessage"] = "Registration successful. Please log in.";
                return RedirectToAction("Login");
            }
            catch
            {
                TempData["ErrorMessage"] = "An error occurred while creating the account.";
                return View(model);
            }
        }

        /// <summary>
        /// Sign Out current account and return login view.
        /// </summary>
        /// <returns>Created response view</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}