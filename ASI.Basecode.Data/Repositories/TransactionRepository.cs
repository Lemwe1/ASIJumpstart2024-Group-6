using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System.Linq;
using System;

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
            // Fetch the existing transaction from the database
            var existingTransaction = await _context.MTransactions.FindAsync(transaction.TransactionId);

            if (existingTransaction != null)
            {
                // Log the existing transaction's wallet ID for debugging
                Console.WriteLine($"Updating Transaction ID: {existingTransaction.TransactionId} with new Wallet ID: {transaction.WalletId}");

                // Update properties explicitly
                existingTransaction.TransactionType = transaction.TransactionType;
                existingTransaction.Amount = transaction.Amount;
                existingTransaction.TransactionDate = transaction.TransactionDate;
                existingTransaction.Note = transaction.Note;
                existingTransaction.CategoryId = transaction.CategoryId;
                existingTransaction.WalletId = transaction.WalletId;
                existingTransaction.TransactionSort = transaction.TransactionSort;

                // Mark the entity as modified
                _context.Entry(existingTransaction).State = EntityState.Modified;

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            else
            {
                // Log if the existing transaction is not found
                Console.Error.WriteLine($"Transaction ID {transaction.TransactionId} not found for update.");
            }
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
