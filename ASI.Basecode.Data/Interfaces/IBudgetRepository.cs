﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBudgetRepository
    {
        Task<IEnumerable<MBudget>> GetBudgetsByUserIdAsync(int userId);
        Task<MBudget> GetByIdAsync(int budgetId); // Fetch a single budget by ID
        Task AddAsync(MBudget budget);
        Task UpdateAsync(MBudget budget);
    }
}