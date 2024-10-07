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
            var debitLiabilities = await _debitLiabilitiesService.GetDebitLiabilitiesAsync();

            // Calculate the total debit and total liabilities
            var totalDebit = debitLiabilities.Where(x => x.DeLiType == "debit").Sum(x => x.DeLiBalance);
            var totalLiabilities = debitLiabilities.Where(x => x.DeLiType == "borrowed").Sum(x => x.DeLiBalance);

            // Calculate net worth
            var netWorth = totalDebit - totalLiabilities;

            // Pass the values to the view using ViewBag or ViewData
            ViewBag.TotalDebit = totalDebit;
            ViewBag.TotalLiabilities = totalLiabilities;
            ViewBag.NetWorth = netWorth;

            return View(debitLiabilities);
        }


        // POST: /DebitLiabilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] MDebitLiab model)
        {
            Console.WriteLine("Request received");  // Log that the request was received

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                Console.WriteLine($"ModelState errors: {string.Join(", ", errors)}");  // Log ModelState errors
                return BadRequest(new { success = false, message = "Invalid data", errors });
            }

            if (string.IsNullOrEmpty(model.DeLiType))
            {
                Console.WriteLine("DeLiType is required.");  // Log missing DeLiType
                return BadRequest(new { success = false, message = "DeLiType is required." });
            }

            try
            {
                Console.WriteLine("Creating new Debit Liability");  // Log success step

                var newDebitLiab = new MDebitLiab
                {
                    DeLiType = model.DeLiType,
                    DeLiIcon = model.DeLiIcon,
                    DeLiColor = model.DeLiColor,
                    DeLiBalance = model.DeLiBalance,
                    DeLiName = model.DeLiName,
                    DeLiHapp = model.DeLiHapp,
                    DeLiDue = model.DeLiDue
                };

                await _debitLiabilitiesService.AddDebitLiabilityAsync(newDebitLiab);
                return Json(new { success = true, message = "Debit Liability created successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");  // Log the exception
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
