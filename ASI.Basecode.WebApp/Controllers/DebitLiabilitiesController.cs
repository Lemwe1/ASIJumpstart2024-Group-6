using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class DebitLiabilitiesController : Controller
    {
        private List<DebitAccount> _debitAccounts = new List<DebitAccount>();
        private List<Liability> _liabilities = new List<Liability>();

        public DebitLiabilitiesController()
        {
            // Sample Data (Replace with Database later)
            _debitAccounts = new List<DebitAccount>
        {
            new DebitAccount { Id = 1, Name = "Cash", Icon = "fas fa-money-bill", Balance = 5000, Color = "green" },
            new DebitAccount { Id = 2, Name = "BPI", Icon = "fas fa-bank", Balance = 3000, Color = "blue" }
        };

            _liabilities = new List<Liability>
        {
            new Liability { Id = 1, Name = "Loan", Amount = 2000, Color = "red" }
        };
        }



        public IActionResult Index()
        {
            var viewModel = new DebitLiabilitiesViewModel
            {
                DebitAccounts = _debitAccounts,
                Liabilities = _liabilities,
                TotalNetWorth = _debitAccounts.Sum(a => a.Balance) - _liabilities.Sum(l => l.Amount)
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(DebitAccount account)
        {
            // Add logic here
            _debitAccounts.Add(account);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var account = _debitAccounts.FirstOrDefault(a => a.Id == id);
            return View(account);
        }

        [HttpPost]
        public IActionResult Edit(DebitAccount account)
        {
            var existingAccount = _debitAccounts.FirstOrDefault(a => a.Id == account.Id);
            if (existingAccount != null)
            {
                existingAccount.Name = account.Name;
                existingAccount.Balance = account.Balance;
                // Update other fields
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var account = _debitAccounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                _debitAccounts.Remove(account);
            }
            return RedirectToAction("Index");
        }
    }

}
