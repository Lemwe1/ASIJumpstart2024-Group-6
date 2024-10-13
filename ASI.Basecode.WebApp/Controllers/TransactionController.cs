using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Mvc;
using ASI.Basecode.WebApp.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class TransactionController : ControllerBase<TransactionController>
    {
        private readonly IDebitLiabilitiesService _debitLiabilitiesService;
        private readonly ICategoryService _categoryService;

        public TransactionController(IHttpContextAccessor httpContextAccessor,
                                     ILoggerFactory loggerFactory,
                                     IConfiguration configuration,
                                     IMapper mapper,
                                     IDebitLiabilitiesService debitLiabilitiesService,
                                     ICategoryService categoryService)
            : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _debitLiabilitiesService = debitLiabilitiesService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Transactions"; // Set title for the transactions page

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdString, out int userId))
            {
                return BadRequest("Invalid user ID.");
            }

            // Fetch categories and debit liabilities
            var categories = await _categoryService.GetCategoriesAsync(userIdString);
            var debitLiabilities = await _debitLiabilitiesService.GetDebitLiabilitiesAsync(userId);

            // Pass data to the view
            ViewData["Categories"] = categories;
            ViewData["DebitLiabilities"] = debitLiabilities.ToList();

            return View();
        }
    }
}
