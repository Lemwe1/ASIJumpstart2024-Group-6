using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class HomeController : ControllerBase<HomeController>
    {
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;
        private readonly IBudgetService _budgetService;
        private readonly ICategoryService _categoryService;

        public HomeController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IWalletService walletService,
                              ITransactionService transactionService,
                              IBudgetService budgetService,
                              ICategoryService categoryService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _budgetService = budgetService ?? throw new ArgumentNullException(nameof(budgetService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        public async Task<IActionResult> Index(bool json = false)
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var wallet = await _walletService.GetWalletAsync(userId);
            var userWallet = wallet.Where(x => x.UserId == userId).ToList();

            var transactions = await _transactionService.GetAllTransactionsAsync(userId);

            var totalIncome = transactions.Where(t => t.TransactionType == "Income" && t.TransactionSort == "Transaction").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.TransactionType == "Expense" && t.TransactionSort == "Transaction").Sum(t => t.Amount);

            var netBalance = totalIncome - totalExpense;

            var budgets = await _budgetService.GetBudgetsAsync(userId);

            ViewBag.NetBalance = netBalance;
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewData["Budgets"] = budgets;

            if (json)
            {
                return Json(new { userWallet, totalIncome, totalExpense, netBalance, budgets });
            }

            return View(userWallet);
        }

        [HttpPost]
        public async Task<IActionResult> AddBudget([FromBody] BudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid budget data." });
            }

            try
            {
                await _budgetService.AddBudgetAsync(model);
                return Ok(new { message = "Budget added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBudget([FromBody] BudgetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid budget data." });
            }

            try
            {
                await _budgetService.UpdateBudgetAsync(model);
                return Ok(new { message = "Budget updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}
