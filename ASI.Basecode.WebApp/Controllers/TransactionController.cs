using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

            return View();
        }

        // POST: Handle the form submission to create a transaction
        [HttpPost]
        public async Task<IActionResult> Create(TransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return BadRequest("Invalid user ID.");
                }

                await LoadDropdownsForUser(userId.Value);
                return View(model);
            }

            // Call the service to add the transaction
            await _transactionService.AddTransactionAsync(model);

            // Redirect to the Index page upon success
            return RedirectToAction("Index");
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

            await LoadDropdownsForUser(userId.Value);

            return View(transaction);
        }

        // POST: Update an existing transaction
        [HttpPost]
        public async Task<IActionResult> Edit(TransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = GetUserId();
                if (userId == null)
                {
                    return BadRequest("Invalid user ID.");
                }

                await LoadDropdownsForUser(userId.Value);
                return View(model);
            }

            // Call the service to update the transaction
            await _transactionService.UpdateTransactionAsync(model);

            // Redirect to the Index page upon success
            return RedirectToAction("Index");
        }

        // POST: Delete a transaction by ID
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _transactionService.DeleteTransactionAsync(id);
            return RedirectToAction("Index");
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
