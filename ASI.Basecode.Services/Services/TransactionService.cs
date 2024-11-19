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
            throw new Exception("Wallet not found.");
        }

        // Adjust wallet balance based on the original balance before transaction changes
        AdjustWalletBalance(wallet, transaction.Amount, transaction.TransactionType, true); // True indicates using original balance

        // Save the wallet update and add the transaction
        await _walletRepository.UpdateAsync(wallet);
        await _transactionRepository.AddAsync(transaction);
    }

    public async Task UpdateTransactionAsync(TransactionViewModel model)
    {
        var transaction = MapToModel(model);
        var existingTransaction = await _transactionRepository.GetByIdAsync(transaction.TransactionId);

        if (existingTransaction == null)
        {
            throw new Exception($"Transaction ID {transaction.TransactionId} not found for update.");
        }

        var previousWalletId = existingTransaction.WalletId;
        var previousWallet = await _walletRepository.GetByIdAsync(previousWalletId);

        if (previousWallet == null)
        {
            throw new Exception("Previous wallet not found.");
        }

        // Undo balance effect if necessary and adjust based on WalletOriginalBalance
        AdjustWalletBalance(previousWallet, -existingTransaction.Amount, existingTransaction.TransactionType, true);

        // If wallet or amount changed, update the wallet balance
        if (transaction.WalletId != previousWalletId)
        {
            var newWallet = await _walletRepository.GetByIdAsync(transaction.WalletId);
            if (newWallet == null)
            {
                throw new Exception("New wallet not found.");
            }

            AdjustWalletBalance(newWallet, transaction.Amount, transaction.TransactionType, false);
            await _walletRepository.UpdateAsync(newWallet);
        }
        else
        {
            // Update the balance for the same wallet by considering the new transaction amount
            var amountDifference = transaction.Amount - existingTransaction.Amount;
            AdjustWalletBalance(previousWallet, amountDifference, transaction.TransactionType, false);
        }

        // Update transaction details
        UpdateTransactionDetails(existingTransaction, transaction);

        // Save the updated transaction
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

        // Undo the effect of the transaction on the wallet balance
        AdjustWalletBalance(wallet, -existingTransaction.Amount, existingTransaction.TransactionType, true);

        // Update the wallet and delete the transaction
        await _walletRepository.UpdateAsync(wallet);
        await _transactionRepository.DeleteAsync(transactionId);
    }

    // Helper method to adjust wallet balance considering WalletOriginalBalance
    private void AdjustWalletBalance(MWallet wallet, decimal amount, string transactionType, bool isInitialAdjustment)
    {
        decimal balance = isInitialAdjustment ? wallet.WalletOriginalBalance : wallet.WalletBalance;

        if (transactionType == "Expense")
        {
            balance -= amount;
        }
        else if (transactionType == "Income")
        {
            balance += amount;
        }

        // Update the WalletBalance or WalletOriginalBalance accordingly
        if (isInitialAdjustment)
        {
            wallet.WalletOriginalBalance = balance;
        }
        else
        {
            wallet.WalletBalance = balance;
        }
    }

    // Helper method to update transaction details
    private void UpdateTransactionDetails(MTransaction existingTransaction, MTransaction newTransaction)
    {
        existingTransaction.TransactionDate = newTransaction.TransactionDate;
        existingTransaction.Note = newTransaction.Note;
        existingTransaction.CategoryId = newTransaction.CategoryId;
        existingTransaction.TransactionSort = newTransaction.TransactionSort;

        if (existingTransaction.TransactionType != newTransaction.TransactionType)
        {
            existingTransaction.TransactionType = newTransaction.TransactionType;
        }
        if (existingTransaction.Amount != newTransaction.Amount)
        {
            existingTransaction.Amount = newTransaction.Amount;
        }
        if (existingTransaction.WalletId != newTransaction.WalletId)
        {
            existingTransaction.WalletId = newTransaction.WalletId;
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