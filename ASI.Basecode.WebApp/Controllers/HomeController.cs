using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : ControllerBase<HomeController>
    {

        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public HomeController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IWalletService walletService,
                               ITransactionService transactionService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _transactionService = transactionService;

        }


        public async Task<IActionResult> Index(bool json = false)
        {
            // Get the logged-in user's ID
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            // Fetch the wallet data for the logged-in user
            var wallet = await _walletService.GetWalletAsync(userId);
            var userWallet = wallet
                .Where(x => x.UserId == userId)
                .ToList(); // Convert to List<DebitLiabilityViewModel>

            // Get all transactions (Income and Expense) for the logged-in user
            var transactions = await _transactionService.GetAllTransactionsAsync(userId);

            // Calculate the totals for Income and Expense
            var totalIncome = transactions.Where(t => t.TransactionType == "Income" && t.TransactionSort == "Transaction").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.TransactionType == "Expense" && t.TransactionSort == "Transaction").Sum(t => t.Amount);

            // Calculate total wallet balance (total debit)
            var totalDebit = userWallet.Sum(x => x.WalletBalance);

            // Add totals to ViewBag to display them in the view
            ViewBag.TotalDebit = totalDebit;
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;

            if (json)
            {
                // Return the wallet and transactions in JSON format if requested
                return Json(new { userWallet, totalIncome, totalExpense });
            }

            // Return the view with wallet data and transaction totals
            return View(userWallet);
        }
    }
}