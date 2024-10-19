using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<IActionResult> Create(TransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Capture detailed validation errors
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                // Log the validation errors
                foreach (var key in errors.Keys)
                {
                    foreach (var error in errors[key])
                    {
                        Console.WriteLine($"Validation Error - {key}: {error}");
                    }
                }

                // Return the validation errors in the response
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }

            // Get the currently logged-in user's ID
            var userClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userClaim == null)
            {
                return Json(new { success = false, message = "User not authenticated." });
            }

            // Parse the user ID and assign it to the model
            model.UserId = int.Parse(userClaim.Value);

            try
            {
                // Proceed with adding the transaction
                await _transactionService.AddTransactionAsync(model);
                return Json(new { success = true, message = "Transaction added successfully!", data = model });
            }
            catch (Exception ex)
            {
                // Log the exception message (optional: use a logging framework)
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        // GET: Edit a transaction by ID
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("Invalid user ID.");
            }

            // Ensure that the transaction belongs to the logged-in user
            if (transaction.UserId != userId.Value)
            {
                return Forbid(); // or return NotFound()
            }

            await LoadDropdownsForUser(userId.Value);

            // Return the existing transaction directly as a model for editing
            return View(transaction); // Assuming the view is expecting the transaction type or it's a suitable model
        }

        // POST: Update an existing transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TransactionViewModel model)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("Invalid user ID.");
            }

            // Ensure the transaction belongs to the current user
            var existingTransaction = await _transactionService.GetTransactionByIdAsync(model.TransactionId);
            if (existingTransaction == null || existingTransaction.UserId != userId)
            {
                return Forbid(); // Prevent unauthorized access
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsForUser(userId.Value); // Load dropdowns if the model is invalid
                return View(existingTransaction); // Return existing transaction if model state is invalid
            }

            // Update the properties of the existing transaction based on the model
            existingTransaction.Amount = model.Amount;
            existingTransaction.TransactionDate = model.TransactionDate;
            existingTransaction.Note = model.Note;
            existingTransaction.CategoryId = model.CategoryId;
            // ... add other properties as necessary

            // Call the service to update the transaction
            await _transactionService.UpdateTransactionAsync(existingTransaction);

            // Redirect to the Index page upon success
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            
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
