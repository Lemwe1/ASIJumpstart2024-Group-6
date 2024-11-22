using System.Linq;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASI.Basecode.Data;
using System.Data.Entity.Infrastructure;

namespace ASI.Basecode.WebApp.Controllers
{
    public class SettingsController : ControllerBase<HomeController>
    {
        private readonly IUserService _userService;
        private readonly ILogger<SettingsController> _logger;
        private readonly AsiBasecodeDbContext _dbContext;

        public SettingsController(IHttpContextAccessor httpContextAccessor,
                                  ILoggerFactory loggerFactory,
                                  IConfiguration configuration,
                                  IMapper mapper,
                                  IUserService userService,
                                  ILogger<SettingsController> logger,
                                  AsiBasecodeDbContext dbContext)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _userService = userService;
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Settings Page";

            // Log all claims to help debug the issue
            var claims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            foreach (var claim in claims)
            {
                _logger.LogInformation($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            var userIdClaim = HttpContext.User.FindFirst("UserId")?.Value;

            if (HttpContext.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                ViewData["UserId"] = userId;
                // Fetch user data using the _userService
                var user = _userService.RetrieveUser(userId);

                if (user != null)
                {
                    _logger.LogInformation($"User found: FirstName = {user.FirstName}, LastName = {user.LastName}, Mail = {user.Mail}");

                    ViewData["UserId"] = user.Id;
                    ViewData["FirstName"] = user.FirstName;
                    ViewData["LastName"] = user.LastName;
                    ViewData["Email"] = user.Mail;
                    ViewData["UserCode"] = user.UserCode;
                }
                else
                {
                    ViewData["Error"] = "User not found.";
                }
            }
            else
            {
                // If the user is not authenticated or user ID is missing, show an error message
                ViewData["Error"] = "User not authenticated or invalid user ID.";
            }

            return View();
        }

        public async Task<IActionResult> EditUserInformation(UserViewModel model)
        {
            // Remove the password field from model state if it's inadvertently added
            ModelState.Remove("Password");

            if (!ModelState.IsValid)
            {
                // Log all model state errors for debugging
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("ModelState error: {Error}", error.ErrorMessage);
                }
                return View("EditProfile", model);
            }

            // Proceed with the update process (excluding password)
            var user = await _dbContext.MUsers.FindAsync(model.Id);
            if (user == null)
            {
                _logger.LogError("User with ID {UserId} not found.", model.Id);
                return NotFound();
            }

            // Log original user data before updating
            _logger.LogInformation("Original User: FirstName: {FirstName}, LastName: {LastName}, Email: {Email}", user.FirstName, user.LastName, user.Mail);

            // Update user data (no password)
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Mail = model.Mail;

            // Log updated user data
            _logger.LogInformation("Updated User: FirstName: {FirstName}, LastName: {LastName}, Email: {Email}", model.FirstName, model.LastName, model.Mail);

            // Save changes to the database
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            // Log successful update
            _logger.LogInformation("User information updated successfully for UserId: {UserId}", model.Id);

            // Redirect with success message
            TempData["SuccessMessage"] = "Your information has been updated successfully!";
            return RedirectToAction("Index", "Settings");
        }
    }
}