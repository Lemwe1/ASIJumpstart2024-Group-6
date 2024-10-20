﻿using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IDebitLiabilitiesService _debitLiabilitiesService;
        private readonly ICategoryService _categoryService;
        private readonly ITransactionService _transactionService;

        public TransactionController(IDebitLiabilitiesService debitLiabilitiesService,
                                     ICategoryService categoryService,
                                     ITransactionService transactionService)
        {
            _debitLiabilitiesService = debitLiabilitiesService;
            _categoryService = categoryService;
            _transactionService = transactionService;
        }

        // GET: Display the list of transactions
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("Invalid user ID.");
            }

            ViewData["Title"] = "Transactions";

            // Fetch categories, debit liabilities, and transactions for the user
            var categories = await _categoryService.GetCategoriesAsync(userId.Value);
            var debitLiabilities = await _debitLiabilitiesService.GetDebitLiabilitiesAsync(userId.Value);
            var transactions = await _transactionService.GetAllTransactionsAsync(userId.Value);

            // Pass data to the view
            ViewData["Categories"] = categories;
            ViewData["DebitLiabilities"] = debitLiabilities.ToList();
            ViewData["Transactions"] = transactions.ToList();

            return View();
        }

        // GET: Display the create transaction form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("Invalid user ID.");
            }

            await LoadDropdownsForUser(userId.Value);
            return View(new TransactionViewModel()); // Ensure we return an empty model for the create view
        }

        // GET: /Transaction/GetTransaction/{id}
        [HttpGet]
        public async Task<IActionResult> GetTransaction(int id)
        {
            // Get user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Fetch the transaction by ID
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null || transaction.UserId.ToString() != userId)
            {
                return NotFound(new { success = false, message = "Transaction not found." });
            }

            // Retrieve category and debit/liability names
            var categoryName = await _categoryService.GetCategoryNameByIdAsync(transaction.CategoryId, int.Parse(userId));
            var debitLiabilityName = await _debitLiabilitiesService.GetDebitLiabilityNameByIdAsync(transaction.DeLiId, int.Parse(userId));

            // Map MTransaction to TransactionViewModel
            var transactionViewModel = new TransactionViewModel
            {
                TransactionId = transaction.TransactionId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Note = transaction.Note,
                CategoryId = transaction.CategoryId,
                DeLiId = transaction.DeLiId,
                CategoryName = categoryName, 
                DebitLiabilityName = debitLiabilityName 
            };

            return Json(new { success = true, data = transactionViewModel });
        }

        // POST: Create a new transaction
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }

            var userClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userClaim == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            model.UserId = int.Parse(userClaim.Value);

            try
            {
                await _transactionService.AddTransactionAsync(model);
                return Json(new { success = true, message = "Transaction added successfully!", data = model });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        // GET: Edit a transaction by ID
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("Invalid user ID.");
            }

            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null || transaction.UserId != userId)
            {
                return NotFound();
            }

            // Map the existing transaction to TransactionViewModel
            var transactionViewModel = new TransactionViewModel
            {
                TransactionId = transaction.TransactionId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Note = transaction.Note,
                CategoryId = transaction.CategoryId,
                DeLiId = transaction.DeLiId // Ensure this is set as well
            };

            // Ensure categories and debit liabilities are loaded
            ViewData["Categories"] = await _categoryService.GetCategoriesAsync(userId.Value); // Pass user ID
            ViewData["DebitLiabilities"] = await _debitLiabilitiesService.GetDebitLiabilitiesAsync(userId.Value); // Pass user ID

            return View(transactionViewModel);
        }

        // POST: EDIT a transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] TransactionViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { success = false, message = "Model is null." });
                }

                // Check for ID mismatch
                if (id != model.TransactionId)
                {
                    return BadRequest(new { success = false, message = "Transaction ID mismatch." });
                }

                var existingTransaction = await _transactionService.GetTransactionByIdAsync(id);
                if (existingTransaction == null)
                {
                    return NotFound(new { success = false, message = "Transaction not found." });
                }

                // Update properties of the existing transaction
                existingTransaction.TransactionType = model.TransactionType;
                existingTransaction.Amount = model.Amount;
                existingTransaction.TransactionDate = model.TransactionDate;
                existingTransaction.Note = model.Note;
                existingTransaction.CategoryId = model.CategoryId;
                existingTransaction.DeLiId = model.DeLiId;

                // Call the service to update the transaction
                await _transactionService.UpdateTransactionAsync(existingTransaction);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.Error.WriteLine($"Error in Edit method: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error", details = ex.Message });
            }
        }


        // POST: Delete a transaction
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("Invalid user ID.");
            }

            var existingTransaction = await _transactionService.GetTransactionByIdAsync(id);
            if (existingTransaction == null || existingTransaction.UserId != userId)
            {
                return NotFound(); // Prevent unauthorized access
            }

            await _transactionService.DeleteTransactionAsync(id);
            return Ok(new { message = "Transaction deleted successfully." });
        }

        // Helper method to get User ID
        private int? GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }
            return null;
        }

        // Helper method to load categories and debit liabilities for the dropdowns
        private async Task LoadDropdownsForUser(int userId)
        {
            var categories = await _categoryService.GetCategoriesAsync(userId);
            var debitLiabilities = await _debitLiabilitiesService.GetDebitLiabilitiesAsync(userId);
            ViewBag.Categories = categories;
            ViewBag.DebitLiabilities = debitLiabilities;
        }
    }
}
