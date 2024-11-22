using System.Collections.Generic;
using System.Threading.Tasks;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBudgetService
    {
        Task<IEnumerable<BudgetViewModel>> GetBudgetsAsync(int userId);
        Task AddBudgetAsync(BudgetViewModel model);
        Task UpdateBudgetAsync(BudgetViewModel model); // Add this method
    }
}