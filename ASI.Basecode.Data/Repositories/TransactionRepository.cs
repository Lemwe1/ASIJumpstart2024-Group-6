using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AsiBasecodeDbContext _context;

        public TransactionRepository(AsiBasecodeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MTransaction>> GetAllAsync()
        {
            // Fetch all transactions asynchronously
            return await _context.MTransactions.ToListAsync();
        }

        public async Task<MTransaction> GetByIdAsync(int transactionId)
        {
            // Fetch a single transaction by ID asynchronously
            return await _context.MTransactions.FindAsync(transactionId);
        }

        public async Task AddAsync(MTransaction transaction)
        {
            // Add a new transaction asynchronously
            await _context.MTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MTransaction transaction)
        {
            // Update an existing transaction asynchronously
            _context.MTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int transactionId)
        {
            // Remove a transaction by ID asynchronously
            var transaction = await _context.MTransactions.FindAsync(transactionId);
            if (transaction != null)
            {
                _context.MTransactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
    }
}
