using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Newtonsoft.Json;

namespace ASI.Basecode.Services.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ITransactionRepository _transactionRepository;

        public BudgetService(IBudgetRepository budgetRepository, ITransactionRepository transactionRepository)
        {
            _budgetRepository = budgetRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<BudgetViewModel>> GetBudgetsAsync(int userId)
        {
            // Get all budgets from the repository including related data
            var budgets = await _budgetRepository.GetBudgetsByUserIdAsync(userId);

            // Filter budgets by userId and map MBudget to BudgetViewModel
            var budgetViewModels = budgets
                .Where(budget => budget.UserId == userId)
                .Select(budget => new BudgetViewModel
                {
                    BudgetId = budget.BudgetId,
                    CategoryId = budget.CategoryId,
                    UserId = budget.UserId,
                    MonthlyBudget = budget.MonthlyBudget,
                    RemainingBudget = budget.RemainingBudget,
                    CategoryName = budget.Category?.Name ?? string.Empty,
                    CategoryIcon = budget.Category?.Icon ?? string.Empty,
                    CategoryColor = budget.Category?.Color ?? string.Empty
                });

            return budgetViewModels;
        }

        public async Task AddBudgetAsync(BudgetViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Budget data is null.");

            if (model.UserId <= 0 || model.CategoryId <= 0 || model.MonthlyBudget <= 0)
                throw new ArgumentException("Invalid budget details.");

            var budget = new MBudget
            {
                BudgetName = $"Budget-{model.CategoryId}", // Temporary name
                CategoryId = model.CategoryId,
                UserId = model.UserId,
                MonthlyBudget = model.MonthlyBudget,
                RemainingBudget = model.MonthlyBudget // Initialize remaining budget
            };

            await _budgetRepository.AddAsync(budget);
        }

        public async Task UpdateBudgetAsync(BudgetViewModel model)
        {
            if (!model.BudgetId.HasValue)
                throw new ArgumentException("Budget ID is required for updating a budget.");

            var existingBudget = await _budgetRepository.GetByIdAsync(model.BudgetId.Value);

            if (existingBudget == null)
                throw new KeyNotFoundException($"Budget with ID {model.BudgetId} not found.");

            // Update properties
            existingBudget.CategoryId = model.CategoryId;
            existingBudget.MonthlyBudget = model.MonthlyBudget;
            existingBudget.RemainingBudget = model.RemainingBudget;

            await _budgetRepository.UpdateAsync(existingBudget);
        }


    }
}