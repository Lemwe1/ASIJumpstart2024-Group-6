using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<IEnumerable<BudgetViewModel>> GetBudgetsAsync(int userId)
        {
            var budgets = await _budgetRepository.GetBudgetsByUserIdAsync(userId);
            return budgets.Select(b => new BudgetViewModel
            {
                BudgetId = b.BudgetId,
                BudgetName = b.BudgetName,
                CategoryId = b.CategoryId,
                UserId = b.UserId,
                MonthlyBudget = b.MonthlyBudget,
                RemainingBudget = b.RemainingBudget,
                CategoryName = b.Category?.Name,
                CategoryIcon = b.Category?.Icon
            });
        }

        public async Task AddBudgetAsync(BudgetViewModel model)
        {
            var budget = new MBudget
            {
                BudgetName = model.BudgetName,
                CategoryId = model.CategoryId,
                UserId = model.UserId,
                MonthlyBudget = model.MonthlyBudget,
                RemainingBudget = model.MonthlyBudget
            };

            await _budgetRepository.AddAsync(budget);
        }

        public async Task UpdateBudgetAsync(BudgetViewModel model)
        {
            var existingBudget = await _budgetRepository.GetByIdAsync(model.BudgetId);

            if (existingBudget == null)
            {
                throw new KeyNotFoundException($"Budget with ID {model.BudgetId} not found.");
            }

            existingBudget.MonthlyBudget = model.MonthlyBudget;
            existingBudget.RemainingBudget = model.RemainingBudget; // Update remaining budget based on logic
            existingBudget.CategoryId = model.CategoryId;

            await _budgetRepository.UpdateAsync(existingBudget);
        }
    }
}
