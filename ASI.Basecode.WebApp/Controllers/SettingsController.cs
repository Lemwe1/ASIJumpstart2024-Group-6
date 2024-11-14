using System.Linq;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASI.Basecode.Services.Interfaces;  
using ASI.Basecode.Services.ServiceModels; 

namespace ASI.Basecode.WebApp.Controllers
{
    public class SettingsController : ControllerBase<HomeController>
    {
        private readonly IUserService _userService;
        private readonly ILogger<SettingsController> _logger;

        public SettingsController(IHttpContextAccessor httpContextAccessor,
                                  ILoggerFactory loggerFactory,
                                  IConfiguration configuration,
                                  IMapper mapper,
                                  IUserService userService,
                                  ILogger<SettingsController> logger)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _userService = userService;
            _logger = logger;
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
                // Fetch user data using the _userService
                var user = _userService.RetrieveUser(userId);

                if (user != null)
                {
                    _logger.LogInformation($"User found: FirstName = {user.FirstName}, LastName = {user.LastName}, Mail = {user.Mail}");

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

    }
}
