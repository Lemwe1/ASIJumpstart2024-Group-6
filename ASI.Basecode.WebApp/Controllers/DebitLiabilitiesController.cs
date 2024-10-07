using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class DebitLiabilitiesController : Controller
    {
        private readonly DebitLiabilitiesService _debitLiabilitiesService;

        public DebitLiabilitiesController(DebitLiabilitiesService debitLiabilitiesService)
        {
            _debitLiabilitiesService = debitLiabilitiesService ?? throw new ArgumentNullException(nameof(debitLiabilitiesService));
        }

        // GET: /DebitLiabilities/
        public async Task<IActionResult> Index()
        {
            // Get the logged-in user's ID
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            // Fetch the debit liabilities belonging to the logged-in user
            var debitLiabilities = await _debitLiabilitiesService.GetDebitLiabilitiesAsync();

            // Filter the list after awaiting the async method
            var userDebitLiabilities = debitLiabilities.Where(x => x.UserId == userId).ToList();

            // Calculate totals
            var totalDebit = userDebitLiabilities.Where(x => x.DeLiType == "debit").Sum(x => x.DeLiBalance);
            var totalLiabilities = userDebitLiabilities.Where(x => x.DeLiType == "borrowed").Sum(x => x.DeLiBalance);
            var netWorth = totalDebit - totalLiabilities;

            ViewBag.TotalDebit = totalDebit;
            ViewBag.TotalLiabilities = totalLiabilities;
            ViewBag.NetWorth = netWorth;

            return View(userDebitLiabilities);
        }

        // POST: /DebitLiabilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] MDebitLiab model)
        {
            Console.WriteLine("Request received");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                Console.WriteLine($"ModelState errors: {string.Join(", ", errors)}");
                return BadRequest(new { success = false, message = "Invalid data", errors });
            }

            if (string.IsNullOrEmpty(model.DeLiType))
            {
                Console.WriteLine("DeLiType is required.");
                return BadRequest(new { success = false, message = "DeLiType is required." });
            }

            try
            {
                Console.WriteLine("Creating new Debit Liability");

                // Get the currently logged-in user's ID (adjust as necessary for your authentication setup)
                int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

                var newDebitLiab = new MDebitLiab
                {
                    DeLiType = model.DeLiType,
                    DeLiIcon = model.DeLiIcon,
                    DeLiColor = model.DeLiColor,
                    DeLiBalance = model.DeLiBalance,
                    DeLiName = model.DeLiName,
                    DeLiHapp = model.DeLiHapp,
                    DeLiDue = model.DeLiDue,
                    UserId = userId // Set the UserId to the logged-in user's ID
                };

                await _debitLiabilitiesService.AddDebitLiabilityAsync(newDebitLiab);
                return Json(new { success = true, message = "Debit Liability created successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        // POST: /DebitLiabilities/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MDebitLiab model)
        {
            if (id != model.DeLiId)
            {
                return BadRequest(new { success = false, message = "Invalid ID." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = "Invalid data", errors });
            }

            try
            {
                // Map ViewModel to Model
                var debitLiability = new MDebitLiab
                {
                    DeLiId = model.DeLiId,
                    DeLiType = model.DeLiType,
                    DeLiIcon = model.DeLiIcon,
                    DeLiColor = model.DeLiColor,
                    DeLiBalance = model.DeLiBalance,
                    DeLiHapp = model.DeLiHapp,
                    DeLiDue = model.DeLiDue
                };

                await _debitLiabilitiesService.UpdateDebitLiabilityAsync(debitLiability);
                return Json(new { success = true, message = "Debit Liability updated successfully." });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new { success = false, message = knfEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: /DebitLiabilities/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _debitLiabilitiesService.DeleteDebitLiabilityAsync(id);
                return Json(new { success = true, message = "Debit Liability deleted successfully." });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new { success = false, message = knfEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
