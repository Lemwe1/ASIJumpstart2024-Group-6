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
using System.Diagnostics;
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
        private readonly IEmailService _emailService;

        public AccountController(
            IEmailService emailService,
            SignInManager signInManager,
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IUserService userService) : base(httpContextAccessor, loggerFactory, configuration)
        {
            this._emailService = emailService;
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
            var encryptedPassword = PasswordManager.EncryptPassword(model.Password);
            var loginResult = _userService.AuthenticateUser(model.UserCode, encryptedPassword, ref user);

            if (loginResult == LoginResult.Success)
            {
                if (!user.isVerified)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Your account is not verified.",
                        resendVerification = true, // Indicate to the front-end that resend link should be displayed
                        email = user.Mail // Pass the email for resending the verification link
                    });
                }

                // Sign in the user
                await _signInManager.SignInAsync(user);
                _session.SetString("UserName", string.Join(" ", user.FirstName, user.LastName));

                // Return success response to the front-end
                return Json(new { success = true, message = "Login successful! Redirecting..." });
            }
            else
            {
                return Json(new { success = false, message = "Incorrect Username or Password" });
            }
        }

        /// <summary>
        /// Register Method - Registers a new user and sends email verification.
        /// </summary>
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
                return Json(new { success = false, message = "Please fill in all required fields." });
            }

            if (model.Password != model.ConfirmPassword)
            {
                return Json(new { success = false, message = "Passwords do not match." });
            }

            // Check if the UserCode already exists in the database
            var existingUserByCode = _userService.GetByUserCode(model.UserCode);
            if (existingUserByCode != null)
            {
                return Json(new { success = false, message = "UserCode is already taken." });
            }

            // Check if the Email already exists in the database
            var existingUserByEmail = _userService.GetByEmail(model.Email);
            if (existingUserByEmail != null)
            {
                return Json(new { success = false, message = "Email is already in use." });
            }

            try
            {
                // Create the new MUser object
                var newUser = new MUser
                {
                    UserCode = model.UserCode,
                    Password = PasswordManager.EncryptPassword(model.Password),  // Use hashed password here
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Mail = model.Email,
                    InsDt = DateTime.Now,
                    VerificationToken = Guid.NewGuid().ToString(),  // Generate token for email verification
                    VerificationTokenExpiration = DateTime.Now.AddHours(24),
                    isVerified = false  // Set account to unverified initially
                };

                // Save the new user to the database
                _userService.Add(newUser);

                // Generate verification link after the user is saved
                var verificationLink = Url.Action("VerifyEmail", "Account", new { token = newUser.VerificationToken }, Request.Scheme);

                // Send email with verification link
                var emailBody = $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.";
                _emailService.SendEmailAsync(newUser.Mail, "Email Verification", emailBody);

                return Json(new { success = true, message = "Registration successful! A verification email has been sent to your email address." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while creating the account. Please try again later." });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "Invalid verification token.";
                return View("VerificationError");
            }

            // Retrieve the user by the verification token
            var user = _userService.GetByVerificationToken(token);

            // Check if the token is valid or expired
            if (user == null || user.VerificationTokenExpiration < DateTime.Now)
            {
                ViewBag.ErrorMessage = "This verification link is invalid or has expired.";
                return View("VerificationError");
            }

            if (user.isVerified)
            {
                ViewBag.ErrorMessage = "This account has already been verified.";
                return View("VerificationError");
            }

            // Mark the account as verified
            user.isVerified = true;
            user.VerificationToken = null;  // Clear the verification token
            user.VerificationTokenExpiration = null;  // Clear the expiration

            // Update the user in the database
            _userService.Update(user);

            // Show success message
            ViewBag.SuccessMessage = "Your email has been verified successfully. You can now log in.";
            return View("VerificationSuccess");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendVerificationLink()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public IActionResult ResendVerificationEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Please provide an email address.";
                return RedirectToAction("ResendVerificationLink");
            }

            var user = _userService.GetByEmail(email);
            if (user == null || user.isVerified)
            {
                TempData["ErrorMessage"] = "Invalid email address or account is already verified.";
                return RedirectToAction("ResendVerificationLink");
            }

            try
            {
                // Generate a new verification token
                user.VerificationToken = Guid.NewGuid().ToString();
                user.VerificationTokenExpiration = DateTime.Now.AddHours(24);
                _userService.Update(user);

                // Generate verification link (no email in the URL)
                var verificationLink = Url.Action("VerifyEmail", "Account", new { token = user.VerificationToken }, Request.Scheme);

                // Send email with verification link
                var emailBody = $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.";
                _emailService.SendEmailAsync(user.Mail, "Email Verification", emailBody);

                TempData["SuccessMessage"] = "A new verification link has been sent to your email.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while sending the verification email. Please try again later.";
                return RedirectToAction("ResendVerificationLink");
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            Debug.WriteLine("ForgotPassword action was hit");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please provide a valid email address.";
                return View();
            }

            var user = _userService.GetByEmail(model.Email);  // Retrieve the user by email
            if (user == null)
            {
                ViewBag.ErrorMessage = "No account found with that email address.";
                return View();
            }

            // Generate a reset token
            var token = Guid.NewGuid().ToString();

            _logger.LogDebug($"Generated token length: {token.Length}");

            // Store the token with an expiration date (e.g., 24 hours)
            user.PasswordResetToken = token;
            user.PasswordResetExpiration = DateTime.Now.AddHours(24);

            _userService.Update(user);  // Save the token and expiration in the database

            // Generate a reset link
            var resetLink = Url.Action("ResetPassword", "Account", new { token = user.PasswordResetToken }, Request.Scheme);

            // Send reset link via email
            var emailBody = $"Click <a href='{resetLink}'>here</a> to reset your password.";
            _emailService.SendEmailAsync(user.Mail, "Password Reset", emailBody);

            // Redirect to the login page after successful email sending
            TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ErrorMessage = "Invalid password reset token.";
                return View("ResetPasswordError");  // Redirect to error view if token is missing
            }

            // Retrieve the user by the token
            var user = _userService.GetByResetToken(token);

            // Check if the token is valid or expired
            if (user == null || user.PasswordResetExpiration < DateTime.Now)
            {
                ViewBag.ErrorMessage = "This password reset link is invalid or has already been used. Please request a new one.";
                return View("ResetPasswordError");  // Redirect to error view if the token is invalid or expired
            }

            // Token is valid, display the reset password form
            var model = new ResetPasswordViewModel { Token = token };
            return View(model);  // Return ResetPassword view
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);  // Return the view with validation messages
            }

            var user = _userService.GetByResetToken(model.Token);
            if (user == null || user.PasswordResetExpiration < DateTime.Now)
            {
                ViewBag.ErrorMessage = "Invalid or expired token.";
                return View(model);
            }

            // Encrypt the new password before saving it to the database
            user.Password = PasswordManager.EncryptPassword(model.NewPassword);
            user.PasswordResetToken = null;  // Clear the reset token
            user.PasswordResetExpiration = null;  // Clear the expiration

            // Update the user in the database
            _userService.Update(user, true); // For password reset

            // Set the success message and render the view with the overlay
            ViewBag.SuccessMessage = "Password has been reset successfully. Redirecting to login...";
            return View();
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
