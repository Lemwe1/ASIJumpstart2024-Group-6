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
            var debitLiabilities = await _debitLiabilitiesService.GetDebitLiabilitiesAsync(userId); // Pass userId here

            // Filter the list after awaiting the async method and convert to a List
            var userDebitLiabilities = debitLiabilities
                .Where(x => x.UserId == userId)
                .ToList(); // Convert to List<MDebitLiab>

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
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                // Get the currently logged-in user's ID
                var userClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userClaim == null)
                {
                    return BadRequest(new { success = false, message = "User not authenticated." });
                }

                int userId = int.Parse(userClaim.Value);
                model.UserId = userId; // Set the UserId to the logged-in user's ID

                // Handle specific properties for borrowed accounts if applicable
                if (model.DeLiType == "borrowed")
                {
                    // You can add further validations if required for borrowed accounts
                    if (model.DeLiHapp == null || model.DeLiDue == null)
                    {
                        return BadRequest(new { success = false, message = "Happening date and due date are required for borrowed accounts." });
                    }
                }

                await _debitLiabilitiesService.AddDebitLiabilityAsync(model);

                return Json(new { success = true, message = "Debit Liability created successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception message (optional: use a logging framework)
                return StatusCode(500, new { success = false, message = $"Server error: {ex.Message}" });
            }
        }


        // POST: /DebitLiabilities/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] MDebitLiab model)
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
                await _debitLiabilitiesService.UpdateDebitLiabilityAsync(model);
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
