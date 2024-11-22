using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.Data.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly AsiBasecodeDbContext _context;

        public BudgetRepository(AsiBasecodeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MBudget>> GetBudgetsByUserIdAsync(int userId)
        {
            return await _context.MBudgets
                .Where(b => b.UserId == userId)
                .Include(b => b.Category) // Include related data
                .ToListAsync();
        }

        public async Task<MBudget> GetByIdAsync(int budgetId)
        {
            return await _context.MBudgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BudgetId == budgetId);
        }

        public async Task AddAsync(MBudget budget)
        {
            if (budget == null)
                throw new ArgumentNullException(nameof(budget));

            await _context.MBudgets.AddAsync(budget);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(MBudget budget)
        {
            _context.MBudgets.Update(budget);
            await _context.SaveChangesAsync();
        }
    }
}