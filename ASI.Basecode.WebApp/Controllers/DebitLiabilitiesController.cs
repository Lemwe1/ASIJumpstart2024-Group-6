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
<<<<<<< HEAD
        private List<DebitAccount> _debitAccounts = new List<DebitAccount>();
        private List<Liability> _liabilities = new List<Liability>();

        public DebitLiabilitiesController()
        {
            // Sample Data (Replace with Database later)
            _debitAccounts = new List<DebitAccount>
        {
            new DebitAccount { DebitId = 1, DebitName = "Cash", DebitIcon = "fas fa-money-bill", DebitBalance = 5000, DebitColor = "green" },
            new DebitAccount { DebitId = 2, DebitName = "BPI", DebitIcon = "fas fa-bank", DebitBalance = 3000, DebitColor = "blue" }
        };

            _liabilities = new List<Liability>
        {
            new Liability { LiabilityId = 1, LiabilityName = "Loan", LiabilityAmount = 2000, LiabilityColor = "red" }
        };
        }



        public IActionResult Index()
        {
            var viewModel = new DebitLiabilitiesViewModel
            {
                DebitAccounts = _debitAccounts,
                Liabilities = _liabilities,
                TotalNetWorth = _debitAccounts.Sum(a => a.DebitBalance) - _liabilities.Sum(l => l.LiabilityAmount)
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
            var account = _debitAccounts.FirstOrDefault(a => a.DebitId == id);
            return View(account);
        }

        [HttpPost]
        public IActionResult Edit(DebitAccount account)
        {
            var existingAccount = _debitAccounts.FirstOrDefault(a => a.DebitId == account.DebitId);
            if (existingAccount != null)
            {
                existingAccount.DebitName = account.DebitName;
                existingAccount.DebitBalance = account.DebitBalance;
                // Update other fields
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var account = _debitAccounts.FirstOrDefault(a => a.DebitId == id);
            if (account != null)
            {
                _debitAccounts.Remove(account);
            }
            return RedirectToAction("Index");
        }
    }

}
=======
      

        public DebitLiabilitiesController()
        {

        }

        

        public IActionResult Index()
        {
           
            return View();
        }
    }

}
>>>>>>> 51c208272ba491f3a0cb176cd8638b9aa32ac5d7
