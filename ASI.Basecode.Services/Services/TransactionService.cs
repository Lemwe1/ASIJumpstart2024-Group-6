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
                WalletName = transaction.Wallet?.WalletName ?? string.Empty
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

        // Check if the wallet exists
        if (wallet == null)
        {
            throw new Exception("Wallet not found");
        }

        // Adjust the wallet balance based on transaction type
        if (transaction.TransactionType == "Expense")
        {
            wallet.WalletBalance -= transaction.Amount; // Deduct for expense
            Console.WriteLine($"Adding Expense: {transaction.Amount}, New Wallet Balance: {wallet.WalletBalance}");
        }
        else if (transaction.TransactionType == "Income")
        {
            wallet.WalletBalance += transaction.Amount; // Add for income
            Console.WriteLine($"Adding Income: {transaction.Amount}, New Wallet Balance: {wallet.WalletBalance}");
        }

        // Save the updated wallet balance
        await _walletRepository.UpdateAsync(wallet);

        // Add the new transaction to the repository
        await _transactionRepository.AddAsync(transaction);
    }

    public async Task UpdateTransactionAsync(TransactionViewModel model)
    {
        // Get the existing transaction to determine the previous amount and type
        var existingTransaction = await _transactionRepository.GetByIdAsync(model.TransactionId);
        if (existingTransaction == null)
        {
            throw new Exception("Transaction not found");
        }

        // Retrieve the associated wallets
        var previousWallet = await _walletRepository.GetByIdAsync(existingTransaction.WalletId);
        var newWallet = await _walletRepository.GetByIdAsync(model.WalletId);

        if (previousWallet == null || newWallet == null)
        {
            throw new Exception("Wallet not found");
        }

        // Adjust the previous wallet's balance based on the existing transaction
        if (existingTransaction.TransactionType == "Expense")
        {
            previousWallet.WalletBalance += existingTransaction.Amount; // Revert original expense deduction
        }
        else if (existingTransaction.TransactionType == "Income")
        {
            previousWallet.WalletBalance -= existingTransaction.Amount; // Revert original income addition
        }

        // Update the transaction properties
        existingTransaction.TransactionType = model.TransactionType;
        existingTransaction.Amount = model.Amount;
        existingTransaction.TransactionDate = model.TransactionDate;
        existingTransaction.Note = model.Note;
        existingTransaction.CategoryId = model.CategoryId;

        // If the WalletId has changed, adjust the new wallet's balance
        if (existingTransaction.WalletId != model.WalletId)
        {
            // Apply the new transaction's impact on the new wallet
            if (model.TransactionType == "Expense")
            {
                newWallet.WalletBalance -= model.Amount; // Deduct for new expense
            }
            else if (model.TransactionType == "Income")
            {
                newWallet.WalletBalance += model.Amount; // Add for new income
            }
        }

        // Save updates for both wallets and the transaction
        await _walletRepository.UpdateAsync(previousWallet);
        if (existingTransaction.WalletId != model.WalletId)
        {
            await _walletRepository.UpdateAsync(newWallet);
        }
        await _transactionRepository.UpdateAsync(existingTransaction);
    }

    public async Task DeleteTransactionAsync(int transactionId)
    {
        // Retrieve the existing transaction to adjust the wallet balance
        var existingTransaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (existingTransaction == null)
        {
            throw new Exception("Transaction not found");
        }

        // Retrieve the associated wallet
        var wallet = await _walletRepository.GetByIdAsync(existingTransaction.WalletId);
        if (wallet == null)
        {
            throw new Exception("Wallet not found");
        }

        // Adjust the wallet balance based on the transaction type
        if (existingTransaction.TransactionType == "Expense")
        {
            wallet.WalletBalance += existingTransaction.Amount; // Return deducted amount
        }
        else if (existingTransaction.TransactionType == "Income")
        {
            wallet.WalletBalance -= existingTransaction.Amount; // Deduct the income amount
        }

        // Save the updated wallet balance
        await _walletRepository.UpdateAsync(wallet);

        // Now delete the transaction
        await _transactionRepository.DeleteAsync(transactionId);
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
            WalletId = viewModel.WalletId,
            UserId = viewModel.UserId,
            TransactionType = viewModel.TransactionType,
            Amount = viewModel.Amount,
            TransactionDate = viewModel.TransactionDate,
            Note = viewModel.Note
        };
    }
}