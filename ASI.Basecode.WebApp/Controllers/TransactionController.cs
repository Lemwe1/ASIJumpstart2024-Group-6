using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Models;
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
        private readonly ITransactionService _transactionService; // Added transaction service

        public TransactionController(IDebitLiabilitiesService debitLiabilitiesService,
                                     ICategoryService categoryService,
                                     ITransactionService transactionService) // Added transaction service to constructor
        {
            _debitLiabilitiesService = debitLiabilitiesService;
            _categoryService = categoryService;
            _transactionService = transactionService; // Initialize the transaction service
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
            var transactions = await _transactionService.GetAllTransactionsAsync(userId); // Fetch transactions

            // Pass data to the view
            ViewData["Categories"] = categories;
            ViewData["DebitLiabilities"] = debitLiabilities.ToList();
            ViewData["Transactions"] = transactions.ToList(); // Pass transactions to the view
            return View();
        }
    }
}
