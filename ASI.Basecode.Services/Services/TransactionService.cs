using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IDebitLiabilitiesRepository _debitLiabilityRepository;

    public TransactionService(ITransactionRepository transactionRepository, IDebitLiabilitiesRepository debitLiabilityRepository)
    {
        _transactionRepository = transactionRepository;
        _debitLiabilityRepository = debitLiabilityRepository;
    }

    public async Task<IEnumerable<TransactionViewModel>> GetAllTransactionsAsync(int userId)
    {
        // Get all transactions for the user from the repository including related data
        var transactions = await _transactionRepository.GetAllAsync();

        // Filter transactions by userId and map MTransaction to TransactionViewModel
        var transactionViewModels = transactions
            .Where(transaction => transaction.UserId == userId)
            .Select(transaction => new TransactionViewModel
            {
                TransactionId = transaction.TransactionId,
                CategoryId = transaction.CategoryId,
                DeLiId = transaction.DeLiId,
                UserId = transaction.UserId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Note = transaction.Note,
                CategoryName = transaction.Category?.Name ?? string.Empty,
                DebitLiabilityName = transaction.DeLi?.DeLiName ?? string.Empty
            });

        return transactionViewModels;
    }

    // Get a specific transaction by ID
    public async Task<TransactionViewModel> GetTransactionByIdAsync(int transactionId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        return transaction != null ? MapToViewModel(transaction) : null;
    }

    // Add a new transaction with deduction/addition logic based on TransactionType
    public async Task AddTransactionAsync(TransactionViewModel transactionViewModel)
    {
        var transaction = MapToModel(transactionViewModel);

        // Check if this transaction is an expense or income and adjust the amount accordingly
        if (transaction.TransactionType == "Expense") // Assuming "Expense" indicates deduction
        {
            // Deduct from the relevant debit liability
            var debitLiability = await _debitLiabilityRepository.GetByIdAsync(transaction.DeLiId);
            if (debitLiability != null)
            {
                debitLiability.DeLiBalance -= transaction.Amount; // Deduct amount for expense
                await _debitLiabilityRepository.UpdateAsync(debitLiability); // Save updated debit liability
            }
        }
        else if (transaction.TransactionType == "Income") // Assuming "Income" indicates addition
        {
            // Add to the relevant debit liability
            var debitLiability = await _debitLiabilityRepository.GetByIdAsync(transaction.DeLiId);
            if (debitLiability != null)
            {
                debitLiability.DeLiBalance += transaction.Amount; // Add amount for income
                await _debitLiabilityRepository.UpdateAsync(debitLiability); // Save updated debit liability
            }
        }

        // Add the new transaction to the repository
        await _transactionRepository.AddAsync(transaction);
    }

    public async Task UpdateTransactionAsync(TransactionViewModel model)
    {
        // Map the view model to the transaction entity (MTransaction)
        var transaction = MapToModel(model);

        // Use the repository to fetch the existing transaction
        var existingTransaction = await _transactionRepository.GetByIdAsync(transaction.TransactionId);

        if (existingTransaction != null)
        {
            // Update properties from the incoming model
            existingTransaction.TransactionType = transaction.TransactionType;
            existingTransaction.Amount = transaction.Amount;
            existingTransaction.TransactionDate = transaction.TransactionDate;
            existingTransaction.Note = transaction.Note;
            existingTransaction.CategoryId = transaction.CategoryId;
            existingTransaction.DeLiId = transaction.DeLiId;

            // Call the repository to update the transaction
            await _transactionRepository.UpdateAsync(existingTransaction);
        }
    }

    // Delete a transaction by ID
    public async Task DeleteTransactionAsync(int transactionId)
    {
        await _transactionRepository.DeleteAsync(transactionId);
    }

    // Mapping from MTransaction to TransactionViewModel
    private TransactionViewModel MapToViewModel(MTransaction model)
    {
        return new TransactionViewModel
        {
            TransactionId = model.TransactionId,
            CategoryId = model.CategoryId,
            DeLiId = model.DeLiId,
            UserId = model.UserId,
            TransactionType = model.TransactionType,
            Amount = model.Amount,
            TransactionDate = model.TransactionDate,
            Note = model.Note
        };
    }

    // Mapping from TransactionViewModel to MTransaction
    private MTransaction MapToModel(TransactionViewModel viewModel)
    {
        return new MTransaction
        {
            TransactionId = viewModel.TransactionId,
            CategoryId = viewModel.CategoryId,
            DeLiId = viewModel.DeLiId,
            UserId = viewModel.UserId,
            TransactionType = viewModel.TransactionType,
            Amount = viewModel.Amount,
            TransactionDate = viewModel.TransactionDate,
            Note = viewModel.Note
        };
    }
}
