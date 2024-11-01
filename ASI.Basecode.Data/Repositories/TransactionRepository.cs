using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System.Linq;

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
            var existingTransaction = await _context.MTransactions.FindAsync(transaction.TransactionId);

            if (existingTransaction != null)
            {
                _context.Entry(existingTransaction).State = EntityState.Detached; // Detach the tracked entity
            }

            // Attach and update the transaction
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
