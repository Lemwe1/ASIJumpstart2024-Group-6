using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Services;
using ASI.Basecode.WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IDebitLiabilitiesService _debitLiabilitiesService;
        private readonly ICategoryService _categoryService;

        public TransactionController(IDebitLiabilitiesService debitLiabilitiesService,
                                     ICategoryService categoryService)
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
            var categories = await _categoryService.GetCategoriesAsync(userId);
            var debitLiabilities = await _debitLiabilitiesService.GetDebitLiabilitiesAsync(userId);

            // Pass data to the view
            ViewData["Categories"] = categories;
            ViewData["DebitLiabilities"] = debitLiabilities.ToList();

            return View();
        }
    }
}