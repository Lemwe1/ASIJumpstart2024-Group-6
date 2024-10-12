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
    public class DebitLiabilitiesController : Controller
    {
        private readonly IDebitLiabilitiesService _debitLiabilitiesService;

        public DebitLiabilitiesController(IDebitLiabilitiesService debitLiabilitiesService)
        {
            _debitLiabilitiesService = debitLiabilitiesService ?? throw new ArgumentNullException(nameof(debitLiabilitiesService));
        }

        // GET: /DebitLiabilities/
        public async Task<IActionResult> Index()
        {
            // Get the logged-in user's ID
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            // Fetch the debit liabilities belonging to the logged-in user
            var debitLiabilities = await _debitLiabilitiesService.GetDebitLiabilitiesAsync(userId);

            // Filter the list after awaiting the async method and convert to a List
            var userDebitLiabilities = debitLiabilities
                .Where(x => x.UserId == userId)
                .ToList(); // Convert to List<DebitLiabilityViewModel>

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
        public async Task<IActionResult> Create([FromBody] DebitLiabilityViewModel model)
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
                model.UserId = userId;

                // Handle specific properties for borrowed accounts if applicable
                if (model.DeLiType == "borrowed" && (model.DeLiHapp == null || model.DeLiDue == null))
                {
                    return BadRequest(new { success = false, message = "Happening date and due date are required for borrowed accounts." });
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
        public async Task<IActionResult> Edit(int id, [FromBody] DebitLiabilityViewModel model)
        {
            if (model == null)
            {
                return BadRequest(new { success = false, message = "Model is null" });
            }

            if (id != model.DeLiId)
            {
                return BadRequest(new { success = false, message = "ID mismatch" });
            }

            // Get the currently logged-in user's ID
            var userClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userClaim == null)
            {
                return BadRequest(new { success = false, message = "User not authenticated." });
            }

            int userId = int.Parse(userClaim.Value);
            model.UserId = userId;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, errors });
            }

            try
            {
                await _debitLiabilitiesService.UpdateDebitLiabilityAsync(model);
                return Json(new { success = true });
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
                // Get user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "User is not authenticated." });
                }

                await _debitLiabilitiesService.DeleteDebitLiabilityAsync(id);
                return Json(new { success = true, message = "Debit liability deleted successfully." });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new { success = false, message = knfEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
