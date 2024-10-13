using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        // Get all transactions
        public async Task<IEnumerable<TransactionViewModel>> GetAllTransactionsAsync(int userId)
        {
            var transactions = await _transactionRepository.GetAllAsync();

            // Map MTransaction to TransactionViewModel
            var transactionViewModels = transactions.Select(t => new TransactionViewModel
            {
                TransactionId = t.TransactionId,
                CategoryId = t.CategoryId,
                DeLiId = t.DeLiId,
                UserId = t.UserId,
                TransactionType = t.TransactionType,
                Description = t.Description,
                Amount = t.Amount,
                TransactionDate = t.TransactionDate,
                Note = t.Note
            }).ToList();

            return transactionViewModels;
        }

        // Get a specific transaction by ID
        public async Task<TransactionViewModel> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            // Map MTransaction to TransactionViewModel
            var transactionViewModel = new TransactionViewModel
            {
                TransactionId = transaction.TransactionId,
                CategoryId = transaction.CategoryId,
                DeLiId = transaction.DeLiId,
                UserId = transaction.UserId,
                TransactionType = transaction.TransactionType,
                Description = transaction.Description,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Note = transaction.Note
            };

            return transactionViewModel;
        }

        // Add a new transaction
        public async Task AddTransactionAsync(TransactionViewModel transactionViewModel)
        {
            // Map TransactionViewModel to MTransaction
            var transaction = new MTransaction
            {
                TransactionId = transactionViewModel.TransactionId,
                CategoryId = transactionViewModel.CategoryId,
                DeLiId = transactionViewModel.DeLiId,
                UserId = transactionViewModel.UserId,
                TransactionType = transactionViewModel.TransactionType,
                Description = transactionViewModel.Description,
                Amount = transactionViewModel.Amount,
                TransactionDate = transactionViewModel.TransactionDate,
                Note = transactionViewModel.Note
            };

            await _transactionRepository.AddAsync(transaction);
        }

        // Update an existing transaction
        public async Task UpdateTransactionAsync(TransactionViewModel transactionViewModel)
        {
            // Map TransactionViewModel to MTransaction
            var transaction = new MTransaction
            {
                TransactionId = transactionViewModel.TransactionId,
                CategoryId = transactionViewModel.CategoryId,
                DeLiId = transactionViewModel.DeLiId,
                UserId = transactionViewModel.UserId,
                TransactionType = transactionViewModel.TransactionType,
                Description = transactionViewModel.Description,
                Amount = transactionViewModel.Amount,
                TransactionDate = transactionViewModel.TransactionDate,
                Note = transactionViewModel.Note
            };

            await _transactionRepository.UpdateAsync(transaction);
        }

        // Delete a transaction by ID
        public async Task DeleteTransactionAsync(int transactionId)
        {
            await _transactionRepository.DeleteAsync(transactionId);
        }
    }
}
