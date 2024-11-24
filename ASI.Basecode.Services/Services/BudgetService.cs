using System;
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

        // Get all budgets for a specific user
        public async Task<IEnumerable<BudgetViewModel>> GetBudgetsAsync(int userId)
        {
            var budgets = await _budgetRepository.GetBudgetsByUserIdAsync(userId);
            return budgets
                .Where(budget => budget.UserId == userId)
                .Select(MapToViewModel); 
        }

        // Get a specific budget by its ID
        public async Task<BudgetViewModel> GetBudgetByIdAsync(int budgetId)
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            return budget != null ? MapToViewModel(budget) : null;
        }

        // Add a new budget
        public async Task AddBudgetAsync(BudgetViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Budget data is null.");

            if (model.UserId <= 0 || model.CategoryId <= 0 || model.MonthlyBudget <= 0)
                throw new ArgumentException("Invalid budget details.");

            var budget = new MBudget
            {
                BudgetName = $"Budget-{model.CategoryId}", // Temporary budget name
                CategoryId = model.CategoryId,
                UserId = model.UserId,
                MonthlyBudget = model.MonthlyBudget,
                RemainingBudget = model.MonthlyBudget 

            };

            await _budgetRepository.AddAsync(budget);
        }

        // Update an existing budget
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

        public async Task DeleteBudgetAsync(int budgetId)
        {
            try
            {
                if (budgetId <= 0)
                    throw new ArgumentException("Invalid budget ID.", nameof(budgetId));

                var existingBudget = await _budgetRepository.GetByIdAsync(budgetId);

                if (existingBudget == null)
                    throw new KeyNotFoundException($"Budget with ID {budgetId} not found.");

                await _budgetRepository.DeleteAsync(budgetId);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
                throw; // Re-throw to allow higher layers to handle it
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Entity not found: {ex.Message}");
                throw; // Re-throw to allow higher layers to handle it
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while deleting budget: {ex.Message}");
                throw; // Re-throw for a 500 response
            }
        }


        // Helper method to map MBudget to BudgetViewModel
        private BudgetViewModel MapToViewModel(MBudget model)
        {
            return new BudgetViewModel
            {
                BudgetId = model.BudgetId,
                CategoryId = model.CategoryId,
                UserId = model.UserId,
                MonthlyBudget = model.MonthlyBudget,
                RemainingBudget = model.RemainingBudget,
                CategoryName = model.Category?.Name ?? string.Empty,
                CategoryIcon = model.Category?.Icon ?? string.Empty,
                CategoryColor = model.Category?.Color ?? string.Empty
            };
        }
    }
}
