using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly ICategoryService _categoryService;
        private readonly ITransactionService _transactionService;

        public TransactionController(IWalletService walletService,
                                     ICategoryService categoryService,
                                     ITransactionService transactionService)
        {
            _walletService = walletService;
            _categoryService = categoryService;
            _transactionService = transactionService;
        }

        // GET: Display the list of transactions
        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("Invalid user ID.");
            }

            // Define how many items per page
            int pageSize = 4;
            int skip = (page - 1) * pageSize;

            ViewData["Title"] = "Transactions";

            // Fetch categories, wallets, and transactions for the user
            var categories = await _categoryService.GetCategoriesAsync(userId.Value);
            var wallets = await _walletService.GetWalletAsync(userId.Value);

            // Get all transactions and apply pagination (limiting to 6 entries for the current page)
            var transactions = await _transactionService.GetAllTransactionsAsync(userId.Value);
            var paginatedTransactions = transactions
                .OrderByDescending(t => t.TransactionId)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            // Get the total count for pagination
            var totalTransactions = transactions.Count();

            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalTransactions / (double)pageSize);

            // Pass data to the view
            ViewData["Categories"] = categories;
            ViewData["Wallets"] = wallets.ToList();
            ViewData["Transactions"] = paginatedTransactions;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;

            // Calculate the number of items displayed on the current page
            int displayedItems = Math.Min(pageSize, totalTransactions - ((page - 1) * pageSize));

            ViewData["DisplayedItems"] = displayedItems; // Pass this to the view
            ViewData["TotalTransactions"] = totalTransactions; // Pass the total number of items to the view

            return View(paginatedTransactions);
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
            return View(new TransactionViewModel());
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

            // Retrieve category and wallets/liability names
            var categoryName = await _categoryService.GetCategoryNameByIdAsync(transaction.CategoryId, int.Parse(userId));
            var walletName = await _walletService.GetWalletNameByIdAsync(transaction.WalletId, int.Parse(userId));

            // Map MTransaction to TransactionViewModel
            var transactionViewModel = new TransactionViewModel
            {
                TransactionId = transaction.TransactionId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Note = transaction.Note,
                CategoryId = transaction.CategoryId,
                WalletId = transaction.WalletId,
                CategoryName = categoryName, 
                WalletName = walletName,
                TransactionSort = transaction.TransactionSort

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
                WalletId = transaction.WalletId,
                TransactionSort = transaction.TransactionSort
            };

            ViewData["Categories"] = await _categoryService.GetCategoriesAsync(userId.Value); 
            ViewData["Wallets"] = await _walletService.GetWalletAsync(userId.Value); 

            return View(transactionViewModel);
        }

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

                if (id != model.TransactionId)
                {
                    return BadRequest(new { success = false, message = "Transaction ID mismatch." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid model state.", errors = ModelState });
                }

                var existingTransaction = await _transactionService.GetTransactionByIdAsync(id);
                if (existingTransaction == null)
                {
                    return NotFound(new { success = false, message = "Transaction not found." });
                }

                // Pass model directly to UpdateTransactionAsync without manually setting properties
                await _transactionService.UpdateTransactionAsync(model);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
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

        // Helper method to load categories and wallets for the dropdowns
        private async Task LoadDropdownsForUser(int userId)
        {
            var categories = await _categoryService.GetCategoriesAsync(userId);
            var wallets = await _walletService.GetWalletAsync(userId);
            ViewBag.Categories = categories;
            ViewBag.Wallets = wallets;
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyTrends()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("Invalid user ID.");
            }

            // Fetch all transactions for the user
            var transactions = await _transactionService.GetAllTransactionsAsync(userId.Value);

            // Filter transactions to only include "Expense" type (string comparison)
            transactions = transactions.Where(t => t.TransactionType == "Expense").ToList();

            // Fetch all categories for the user to map CategoryId to CategoryName
            var categories = await _categoryService.GetCategoriesAsync(userId.Value);
            var categoryMap = categories.ToDictionary(c => c.CategoryId, c => c.Name);

            // Group transactions by month and category, then calculate totals
            var trends = transactions
                .GroupBy(t => new { Month = t.TransactionDate.ToString("yyyy-MM"), t.CategoryId }) // Group by month and category
                .Select(g => new
                {
                    Month = g.Key.Month,
                    CategoryId = g.Key.CategoryId,
                    CategoryName = categoryMap.ContainsKey(g.Key.CategoryId) ? categoryMap[g.Key.CategoryId] : "Unknown",
                    TotalAmount = g.Sum(t => t.Amount)  // Sum up amounts per category per month
                })
                .OrderBy(x => x.Month)
                .ThenBy(x => x.CategoryName)
                .ToList();

            return Json(trends);
        }

        [HttpGet]
        public async Task<IActionResult> GetWeeklyTrends()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("Invalid user ID.");
            }

            var transactions = await _transactionService.GetAllTransactionsAsync(userId.Value);

            // Filter transactions to only include "Expense" type (string comparison)
            transactions = transactions.Where(t => t.TransactionType == "Expense").ToList();

            var categories = await _categoryService.GetCategoriesAsync(userId.Value);
            var categoryMap = categories.ToDictionary(c => c.CategoryId, c => c.Name);

            // Define the order of days (Monday to Sunday)
            var dayOrder = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

            // Get the list of categories that are actually used in the transactions
            var usedCategories = transactions
                .GroupBy(t => t.CategoryId)
                .Select(g => g.Key)
                .ToList();

            // Group transactions by day of the week and category
            var trends = transactions
                .Where(t => usedCategories.Contains(t.CategoryId)) // Only include categories present in transactions
                .GroupBy(t => new
                {
                    DayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(t.TransactionDate.DayOfWeek), // Get day abbreviation
                    t.CategoryId
                })
                .Select(g => new
                {
                    Day = g.Key.DayOfWeek,  // "Mon", "Tue", etc.
                    CategoryId = g.Key.CategoryId,
                    CategoryName = categoryMap.ContainsKey(g.Key.CategoryId) ? categoryMap[g.Key.CategoryId] : "Unknown",
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .OrderBy(x => Array.IndexOf(dayOrder, x.Day)) // Sort days properly
                .ThenBy(x => x.CategoryName) // Sort categories alphabetically
                .ToList();

            // Ensure all days are present, with default values for missing data
            var completeTrends = dayOrder
                .SelectMany(day => categories
                    .Where(c => usedCategories.Contains(c.CategoryId)) // Only include categories used in transactions
                    .Select(c => new
                    {
                        Day = day,
                        CategoryId = c.CategoryId,
                        CategoryName = c.Name,
                        TotalAmount = trends
                            .FirstOrDefault(t => t.Day == day && t.CategoryId == c.CategoryId)?.TotalAmount ?? 0 // Handle missing data
                    }))
                .OrderBy(x => Array.IndexOf(dayOrder, x.Day))
                .ThenBy(x => x.CategoryName)
                .ToList();

            return Json(completeTrends);
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyExpense()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest(new { Error = "Invalid user ID." });
            }

            var transactions = await _transactionService.GetAllTransactionsAsync(userId.Value);

            var monthlyExpenses = transactions
                .Where(t => t.TransactionType == "Expense")
                .GroupBy(t => t.TransactionDate.ToString("yyyy-MM"))
                .Select(g => new
                {
                    Month = g.Key,
                    TotalExpense = g.Sum(t => t.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return Json(new
            {
                TotalExpense = transactions
                    .Where(t => t.TransactionType == "Expense")
                    .Sum(t => t.Amount),
                MonthlyData = monthlyExpenses // Always an array
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyIncome()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest(new { Error = "Invalid user ID." });
            }

            var transactions = await _transactionService.GetAllTransactionsAsync(userId.Value);

            var monthlyIncome = transactions
                .Where(t => t.TransactionType == "Income")
                .GroupBy(t => t.TransactionDate.ToString("yyyy-MM"))
                .Select(g => new
                {
                    Month = g.Key,
                    TotalIncome = g.Sum(t => t.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return Json(new
            {
                TotalIncome = transactions
                    .Where(t => t.TransactionType == "Income")
                    .Sum(t => t.Amount),
                MonthlyData = monthlyIncome // Always an array
            });
        }
    }
}
