using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IBudgetService _budgetService;

    public TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, IBudgetService budgetService)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _budgetService = budgetService;
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
                WalletId = transaction.WalletId,
                UserId = transaction.UserId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Note = transaction.Note,
                CategoryName = transaction.Category?.Name ?? string.Empty,
                WalletName = transaction.Wallet?.WalletName ?? string.Empty,
                CategoryIcon = transaction.Category?.Icon ?? string.Empty,
                WalletIcon = transaction.Wallet?.WalletIcon ?? string.Empty,
                CategoryColor = transaction.Category?.Color ?? string.Empty,
                WalletColor = transaction.Wallet?.WalletColor ?? string.Empty,
                TransactionSort = transaction.TransactionSort
            });

        return transactionViewModels;
    }

    // Get a specific transaction by ID
    public async Task<TransactionViewModel> GetTransactionByIdAsync(int transactionId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        return transaction != null ? MapToViewModel(transaction) : null;
    }

    public async Task AddTransactionAsync(TransactionViewModel transactionViewModel)
    {
        var transaction = MapToModel(transactionViewModel);
        var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId);

        if (wallet == null)
        {
            throw new Exception("Wallet not found.");
        }

        // Adjust wallet balance for the new transaction
        AdjustWalletBalance(wallet, transaction.Amount, transaction.TransactionType);

        // Save changes
        await _walletRepository.UpdateAsync(wallet);
        await _transactionRepository.AddAsync(transaction);
    }

    public async Task UpdateTransactionAsync(TransactionViewModel model)
    {
        var newTransaction = MapToModel(model);
        var existingTransaction = await _transactionRepository.GetByIdAsync(newTransaction.TransactionId);

        if (existingTransaction == null)
        {
            throw new Exception($"Transaction ID {newTransaction.TransactionId} not found.");
        }

        var wallet = await _walletRepository.GetByIdAsync(existingTransaction.WalletId);

        if (wallet == null)
        {
            throw new Exception("Wallet not found.");
        }

        // Undo the previous transaction's effect
        AdjustWalletBalance(wallet, -existingTransaction.Amount, existingTransaction.TransactionType);

        // If the wallet has changed, adjust the new wallet
        if (newTransaction.WalletId != existingTransaction.WalletId)
        {
            var newWallet = await _walletRepository.GetByIdAsync(newTransaction.WalletId);
            if (newWallet == null)
            {
                throw new Exception("New wallet not found.");
            }

            // Apply the new transaction's effect to the new wallet
            AdjustWalletBalance(newWallet, newTransaction.Amount, newTransaction.TransactionType);
            await _walletRepository.UpdateAsync(newWallet);
        }
        else
        {
            // Apply the updated transaction's effect to the same wallet
            AdjustWalletBalance(wallet, newTransaction.Amount, newTransaction.TransactionType);
        }

        // Update the transaction details
        UpdateTransactionDetails(existingTransaction, newTransaction);
        await _transactionRepository.UpdateAsync(existingTransaction);
    }

    public async Task DeleteTransactionAsync(int transactionId)
    {
        var existingTransaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (existingTransaction == null)
        {
            throw new Exception("Transaction not found.");
        }

        var wallet = await _walletRepository.GetByIdAsync(existingTransaction.WalletId);
        if (wallet == null)
        {
            throw new Exception("Wallet not found.");
        }

        // Undo the transaction's effect on the wallet balance
        AdjustWalletBalance(wallet, -existingTransaction.Amount, existingTransaction.TransactionType);

        // Save changes
        await _walletRepository.UpdateAsync(wallet);
        await _transactionRepository.DeleteAsync(transactionId);
    }

    // Helper method to adjust wallet balance
    private void AdjustWalletBalance(MWallet wallet, decimal amount, string transactionType)
    {
        if (transactionType == "Expense")
        {
            wallet.WalletBalance -= amount;
        }
        else if (transactionType == "Income")
        {
            wallet.WalletBalance += amount;
        }
    }

    // Helper method to update transaction details
    private void UpdateTransactionDetails(MTransaction existingTransaction, MTransaction newTransaction)
    {
        existingTransaction.TransactionDate = newTransaction.TransactionDate;
        existingTransaction.Note = newTransaction.Note;
        existingTransaction.CategoryId = newTransaction.CategoryId;
        existingTransaction.TransactionSort = newTransaction.TransactionSort;
        existingTransaction.TransactionType = newTransaction.TransactionType;
        existingTransaction.Amount = newTransaction.Amount;
        existingTransaction.WalletId = newTransaction.WalletId;
    }
    // Mapping from MTransaction to TransactionViewModel
    private TransactionViewModel MapToViewModel(MTransaction model)
    {
        return new TransactionViewModel
        {
            TransactionId = model.TransactionId,
            CategoryId = model.CategoryId,
            WalletId = model.WalletId,
            UserId = model.UserId,
            TransactionType = model.TransactionType,
            Amount = model.Amount,
            TransactionDate = model.TransactionDate,
            Note = model.Note,
            TransactionSort = model.TransactionSort
        };
    }

    // Mapping from TransactionViewModel to MTransaction
    private MTransaction MapToModel(TransactionViewModel viewModel)
    {
        return new MTransaction
        {
            TransactionId = viewModel.TransactionId,
            CategoryId = viewModel.CategoryId,
            WalletId = viewModel.WalletId,
            UserId = viewModel.UserId,
            TransactionType = viewModel.TransactionType,
            Amount = viewModel.Amount,
            TransactionDate = viewModel.TransactionDate,
            Note = viewModel.Note,
            TransactionSort = viewModel.TransactionSort

        };
    }
}