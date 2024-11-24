using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;
        private readonly IBudgetService _budgetService;
        private readonly ICategoryService _categoryService;

        public HomeController(
            IWalletService walletService,
            ITransactionService transactionService,
            IBudgetService budgetService,
            ICategoryService categoryService)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _budgetService = budgetService ?? throw new ArgumentNullException(nameof(budgetService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        // GET: /Home/
        public async Task<IActionResult> Index(bool json = false)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated." });
            }

            // Fetch wallets, transactions, and budgets
            var wallets = await _walletService.GetWalletAsync(userId.Value);
            var transactions = await _transactionService.GetAllTransactionsAsync(userId.Value);
            var budgets = await _budgetService.GetBudgetsAsync(userId.Value); // Fetch budgets
            var categories = await _categoryService.GetCategoriesAsync(userId.Value);

            // Fetch budgets and pass them to the view
            var budgetViewModels = budgets.Select(b => new BudgetViewModel
            {
                BudgetId = b.BudgetId,
                CategoryId = b.CategoryId,
                CategoryName = b.CategoryName, 
                CategoryIcon = b.CategoryIcon, 
                CategoryColor = b.CategoryColor, 
                UserId = b.UserId,
                MonthlyBudget = b.MonthlyBudget,
                RemainingBudget = b.MonthlyBudget - transactions
                    .Where(t => t.CategoryId == b.CategoryId && t.TransactionType == "Expense")
                    .Sum(t => t.Amount)
            })
                .OrderByDescending(b => b.BudgetId)
                .ToList();

            // Pass the budgets and categories to the view
            ViewData["Budgets"] = budgetViewModels;
            ViewData["Categories"] = categories;

            // Calculate totals
            var totalIncome = transactions.Where(t => t.TransactionType == "Income").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.TransactionType == "Expense").Sum(t => t.Amount);
            var netBalance = totalIncome - totalExpense;

            // Pass totals to ViewBag
            ViewBag.NetBalance = netBalance;
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;

            // Return JSON or view
            if (json)
            {
                return Json(new { wallets, totalIncome, totalExpense, netBalance, budgets = budgetViewModels, categories });
            }

            return View(wallets); // Ensure the correct model is passed to the view
        }


        [HttpGet]
        // GET: /Home/GetBudget/{id}
        public async Task<IActionResult> GetBudget(int id)
        {
            try
            {
                // Get the logged-in user's ID
                var userId = GetUserId();
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "User not authenticated." });
                }

                // Fetch the budget by ID
                var budget = await _budgetService.GetBudgetByIdAsync(id);
                if (budget == null || budget.UserId != userId.Value)
                {
                    return NotFound(new { success = false, message = "Budget not found or does not belong to the user." });
                }

                // Map budget to BudgetViewModel
                var budgetViewModel = new BudgetViewModel
                {
                    BudgetId = budget.BudgetId,
                    CategoryId = budget.CategoryId,
                    CategoryName = budget.CategoryName,
                    MonthlyBudget = budget.MonthlyBudget,
                    RemainingBudget = budget.RemainingBudget,
                    CategoryIcon = budget.CategoryIcon,
                    CategoryColor = budget.CategoryColor
                };

                return Json(new { success = true, data = budgetViewModel });
            }
            catch (KeyNotFoundException ex)
            {
                // Handle specific exceptions like not found
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        // POST: /Home/AddBudget
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBudget([FromBody] BudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid budget data.",
                    errors
                });
            }

            try
            {
                // Fetch the logged-in user's ID from claims
                var userClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userClaim == null)
                {
                    return BadRequest(new { success = false, message = "User not authenticated." });
                }

                model.UserId = int.Parse(userClaim.Value);

                // Call the service to add the budget
                await _budgetService.AddBudgetAsync(model);

                return Json(new { success = true, message = "Budget added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }


        // POST: /Home/UpdateBudget
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBudget([FromBody] BudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid budget data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                // Get the currently logged-in user's ID
                var userId = GetUserId();
                if (userId == null)
                {
                    return BadRequest(new { success = false, message = "User not authenticated." });
                }

                model.UserId = userId.Value;

                await _budgetService.UpdateBudgetAsync(model);
                return Json(new { success = true, message = "Budget updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            try
            {
                await _budgetService.DeleteBudgetAsync(id);
                return Ok(new { success = true, message = "Budget deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, new { success = false, message = "An internal server error occurred." });
            }
        }


        // Helper method to get the logged-in user's ID
        private int? GetUserId()
        {
            var userClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userClaim == null)
            {
                return null;
            }

            return int.TryParse(userClaim.Value, out var userId) ? userId : (int?)null;
        }
    }
}