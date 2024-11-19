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

    public TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
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
            throw new Exception("Wallet not found");
        }

        // If this is the first transaction, set the WalletOriginalBalance to the current WalletBalance
        if (wallet.WalletOriginalBalance == 0)
        {
            wallet.WalletOriginalBalance = wallet.WalletBalance;
        }

        // Adjust the wallet balance based on the transaction type (Expense/Income)
        AdjustWalletBalance(wallet, transaction.Amount, transaction.TransactionType);

        // After adjusting balance, update WalletOriginalBalance if needed
        if (wallet.WalletBalance != wallet.WalletOriginalBalance)
        {
            wallet.WalletOriginalBalance = wallet.WalletBalance;
        }

        await _walletRepository.UpdateAsync(wallet);
        await _transactionRepository.AddAsync(transaction);
    }

    public async Task UpdateTransactionAsync(TransactionViewModel model)
    {
        var existingTransaction = await _transactionRepository.GetByIdAsync(model.TransactionId);

        if (existingTransaction == null)
        {
            Console.Error.WriteLine($"Transaction ID {model.TransactionId} not found for update.");
            return;
        }

        var previousWalletId = existingTransaction.WalletId;
        var previousWallet = await _walletRepository.GetByIdAsync(previousWalletId);

        if (previousWallet == null)
        {
            throw new Exception("Previous wallet not found");
        }

        // Adjust previous wallet balance for the transaction removal
        AdjustWalletBalance(previousWallet, -existingTransaction.Amount, existingTransaction.TransactionType);

        // Update the existing transaction's properties
        existingTransaction.TransactionType = model.TransactionType;
        existingTransaction.TransactionDate = model.TransactionDate;
        existingTransaction.Note = model.Note;
        existingTransaction.CategoryId = model.CategoryId;
        existingTransaction.WalletId = model.WalletId;
        existingTransaction.TransactionSort = model.TransactionSort;
        existingTransaction.Amount = model.Amount;  // Ensure amount is updated here

        if (model.WalletId != previousWalletId)
        {
            var newWallet = await _walletRepository.GetByIdAsync(model.WalletId);
            if (newWallet == null)
            {
                throw new Exception("New wallet not found");
            }

            // Adjust new wallet balance after transaction update
            AdjustWalletBalance(newWallet, model.Amount, model.TransactionType);
            await _walletRepository.UpdateAsync(newWallet);
        }
        else
        {
            var amountChange = model.Amount - existingTransaction.Amount;
            // Adjust previous wallet balance based on the amount change
            AdjustWalletBalance(previousWallet, amountChange, model.TransactionType);
        }

        await _walletRepository.UpdateAsync(previousWallet);
        await _transactionRepository.UpdateAsync(existingTransaction);
    }

    public async Task DeleteTransactionAsync(int transactionId)
    {
        var existingTransaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (existingTransaction == null)
        {
            throw new Exception("Transaction not found");
        }

        var wallet = await _walletRepository.GetByIdAsync(existingTransaction.WalletId);
        if (wallet == null)
        {
            throw new Exception("Wallet not found");
        }

        AdjustWalletBalance(wallet, -existingTransaction.Amount, existingTransaction.TransactionType);

        await _walletRepository.UpdateAsync(wallet);
        await _transactionRepository.DeleteAsync(transactionId);
    }

    // Helper method to adjust wallet balance based on transaction type
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