using Microsoft.AspNetCore.Mvc;
using DebitLiabilities.Models;
using System.Collections.Generic;
using ASI.Basecode.WebApp.Models;

public class DebitController : Controller
{
    private static List<DebitModel> debits = new List<DebitModel>();
    private static List<LiabilityModel> liabilities = new List<LiabilityModel>();

    // Display debit list, liabilities, and net worth
    public IActionResult Index()
    {
        // Calculate net worth (Debits - Liabilities)
        decimal totalBalance = 0;
        foreach (var debit in debits)
        {
            totalBalance += debit.Balance;
        }

        decimal totalLiabilities = 0;
        foreach (var liability in liabilities)
        {
            totalLiabilities += liability.Amount;
        }

        ViewBag.NetWorth = totalBalance - totalLiabilities;
        ViewBag.Debits = debits;
        ViewBag.Liabilities = liabilities;

        return View();
    }

    // GET: Add new debit
    [HttpGet]
    public IActionResult AddDebit()
    {
        return View();
    }

    // POST: Save new debit
    [HttpPost]
    public IActionResult AddDebit(DebitModel debit)
    {
        if (ModelState.IsValid)
        {
            debits.Add(debit);
            return RedirectToAction("Index");
        }
        return View(debit);
    }

    // GET: Edit debit
    [HttpGet]
    public IActionResult EditDebit(int id)
    {
        var debit = debits.Find(d => d.Id == id);
        return View(debit);
    }

    // POST: Save changes to debit
    [HttpPost]
    public IActionResult EditDebit(DebitModel debit)
    {
        if (ModelState.IsValid)
        {
            var existingDebit = debits.Find(d => d.Id == debit.Id);
            existingDebit.Name = debit.Name;
            existingDebit.Balance = debit.Balance;
            existingDebit.Color = debit.Color;
            return RedirectToAction("Index");
        }
        return View(debit);
    }

    // POST: Delete debit
    [HttpPost]
    public IActionResult DeleteDebit(int id)
    {
        var debit = debits.Find(d => d.Id == id);
        debits.Remove(debit);
        return RedirectToAction("Index");
    }
    

}
