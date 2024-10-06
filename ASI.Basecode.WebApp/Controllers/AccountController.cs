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
using System;
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
        public async Task<JsonResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Please provide valid credentials." });
            }

            MUser user = null;
            var loginResult = _userService.AuthenticateUser(model.UserCode, model.Password, ref user);

            if (loginResult == LoginResult.Success)
            {
                // Sign in the user
                await this._signInManager.SignInAsync(user);
                this._session.SetString("UserName", string.Join(" ", user.FirstName, user.LastName));

                // Return success response to the front-end
                return Json(new { success = true, message = "Login successful! Redirecting..."});
                }
            else
            {
                return Json(new { success = false, message = "Incorrect Username or Password" });
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
        public JsonResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return a JSON response indicating failure and validation error
                return Json(new { success = false, message = "Please fill in all required fields." });
            }

            if (model.Password != model.ConfirmPassword)
            {
                // Return a JSON response indicating failure due to password mismatch
                return Json(new { success = false, message = "Passwords do not match." });
            }

            // Check if the user code is already taken
            if (_userService.RetrieveAll(null, model.UserCode).Any())
            {
                // Return a JSON response indicating failure due to existing user code
                return Json(new { success = false, message = "UserCode is already taken." });
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

                // Return a JSON response indicating success
                return Json(new { success = true, message = "Registration successful! Redirecting to login..." });
            }
            catch (Exception ex)
            {
                // Return a JSON response indicating failure due to an exception
                return Json(new { success = false, message = "An error occurred while creating the account. Please try again later." });
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