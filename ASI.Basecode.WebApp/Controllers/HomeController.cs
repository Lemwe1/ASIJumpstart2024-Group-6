// File: ASI.Basecode.WebApp/Controllers/HomeController.cs
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : ControllerBase<HomeController>
    {
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="transactionService"></param>
        public HomeController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              ITransactionService transactionService = null)
            : base(httpContextAccessor, loggerFactory, configuration)
        {
            _transactionService = transactionService; // Initialize the transaction service
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Home Page"; // Set the title for the home page
            return View();
        }

        // GET: /Home/Transactions
        public IActionResult Transactions()
        {
            var transactions = _transactionService.GetAllTransactions(); // Fetch all transactions
            return View(transactions); // Pass transactions to the view
        }

        // GET: /Home/CreateTransaction
        public IActionResult CreateTransaction()
        {
            return View();
        }

        // POST: /Home/CreateTransaction
        [HttpPost]
        public IActionResult CreateTransaction(TransactionViewModel transactionViewModel)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                _transactionService.CreateTransaction(transactionViewModel); // Call the service to create the transaction
                return RedirectToAction("Transactions"); // Redirect to the transactions page
            }

            return View(transactionViewModel); // Return to the view with the current model
        }

        // GET: /Home/EditTransaction/{id}
        public IActionResult EditTransaction(int id)
        {
            var transactionViewModel = _transactionService.GetTransactionById(id); // Fetch the transaction as a view model
            if (transactionViewModel == null)
            {
                return NotFound(); // Return 404 if not found
            }

            return View(transactionViewModel); // Return the view model for editing
        }

        // POST: /Home/EditTransaction
        [HttpPost]
        public IActionResult EditTransaction(TransactionViewModel transactionViewModel)
        {
            if (ModelState.IsValid) // Check if the model state is valid
            {
                _transactionService.UpdateTransaction(transactionViewModel); // Call the service to update the transaction
                return RedirectToAction("Transactions"); // Redirect to the transactions page
            }

            return View(transactionViewModel); // Return to the view with the current model
        }

        // POST: /Home/DeleteTransaction/{id}
        [HttpPost]
        public IActionResult DeleteTransaction(int id)
        {
            _transactionService.DeleteTransaction(id); // Call the service to delete the transaction
            return RedirectToAction("Transactions"); // Redirect to the transactions page
        }
    }
}
