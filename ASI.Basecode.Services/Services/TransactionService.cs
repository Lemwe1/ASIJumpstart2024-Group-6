// File: ASI.Basecode.Services/Services/TransactionService.cs
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<MTransaction> GetAllTransactions()
        {
            return _repository.RetrieveAll(); // Retrieve all transactions
        }

        public TransactionViewModel GetTransactionById(int id)
        {
            var transaction = _repository.GetTransactionById(id);

            // Map MTransaction to TransactionViewModel
            var transactionViewModel = new TransactionViewModel
            {
                TransactionId = transaction.TransactionId,
                CategoryId = transaction.CategoryId,
                DeLiId = transaction.DeLiId,
                UserId = transaction.UserId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Description = transaction.Description,
                Note = transaction.Note
            };

            return transactionViewModel; // Return the mapped view model
        }

        public void CreateTransaction(TransactionViewModel transactionViewModel)
        {
            var transaction = new MTransaction
            {
                CategoryId = transactionViewModel.CategoryId,
                DeLiId = transactionViewModel.DeLiId,
                UserId = transactionViewModel.UserId,
                TransactionType = transactionViewModel.TransactionType,
                Amount = transactionViewModel.Amount,
                TransactionDate = transactionViewModel.TransactionDate,
                Description = transactionViewModel.Description,
                Note = transactionViewModel.Note
            };

            _repository.Add(transaction); // Add the new transaction
        }

        public void UpdateTransaction(TransactionViewModel transactionViewModel)
        {
            var transaction = new MTransaction
            {
                TransactionId = transactionViewModel.TransactionId,
                CategoryId = transactionViewModel.CategoryId,
                DeLiId = transactionViewModel.DeLiId,
                UserId = transactionViewModel.UserId,
                TransactionType = transactionViewModel.TransactionType,
                Amount = transactionViewModel.Amount,
                TransactionDate = transactionViewModel.TransactionDate,
                Description = transactionViewModel.Description,
                Note = transactionViewModel.Note
            };

            _repository.Update(transaction); // Update the existing transaction
        }

        public void DeleteTransaction(int id)
        {
            _repository.Delete(id); // Delete the transaction by ID
        }
    }
}
