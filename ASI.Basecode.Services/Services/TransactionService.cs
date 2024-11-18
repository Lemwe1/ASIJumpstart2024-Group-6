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

        AdjustWalletBalance(wallet, transaction.Amount, transaction.TransactionType);

        // Update wallet and save the transaction
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

        // Check if all fields are unchanged
        var isAmountUnchanged = transaction.Amount == existingTransaction.Amount;
        var isTransactionTypeUnchanged = transaction.TransactionType == existingTransaction.TransactionType;
        var isWalletIdUnchanged = transaction.WalletId == existingTransaction.WalletId;
        var isTransactionDateUnchanged = transaction.TransactionDate == existingTransaction.TransactionDate;
        var isNoteUnchanged = transaction.Note == existingTransaction.Note;
        var isCategoryIdUnchanged = transaction.CategoryId == existingTransaction.CategoryId;
        var isTransactionSortUnchanged = transaction.TransactionSort == existingTransaction.TransactionSort;

        if (isAmountUnchanged && isTransactionTypeUnchanged && isWalletIdUnchanged &&
            isTransactionDateUnchanged && isNoteUnchanged && isCategoryIdUnchanged && isTransactionSortUnchanged)
        {
            // No changes, skip the update
            return;
        }

        // Proceed with wallet balance adjustment if wallet-related fields are changed
        if (!isAmountUnchanged || !isTransactionTypeUnchanged || !isWalletIdUnchanged)
        {
            // Undo the effect of the original transaction on the previous wallet
            AdjustWalletBalance(previousWallet, -existingTransaction.Amount, existingTransaction.TransactionType);

            // Handle wallet change
            if (!isWalletIdUnchanged)
            {
                var newWallet = await _walletRepository.GetByIdAsync(transaction.WalletId);
                if (newWallet == null)
                {
                    throw new Exception("New wallet not found.");
                }

                // Apply the new transaction effect to the new wallet (for Income or Expense)
                AdjustWalletBalance(newWallet, transaction.Amount, transaction.TransactionType);
                await _walletRepository.UpdateAsync(newWallet);
            }
            else
            {
                // Calculate and apply the delta for the unchanged wallet
                var amountDifference = transaction.Amount - existingTransaction.Amount;
                AdjustWalletBalance(previousWallet, amountDifference, transaction.TransactionType);
            }

            // Update the previous wallet after adjustments
            await _walletRepository.UpdateAsync(previousWallet);
        }

        // Update the transaction fields
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

        // Revert the transaction effect on the wallet balance
        AdjustWalletBalance(wallet, -existingTransaction.Amount, existingTransaction.TransactionType);

        // Update the wallet and delete the transaction
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

    // Helper method to update transaction details
    private void UpdateTransactionDetails(MTransaction existingTransaction, MTransaction newTransaction)
    {
        // Only update non-wallet-related fields
        existingTransaction.TransactionDate = newTransaction.TransactionDate;
        existingTransaction.Note = newTransaction.Note;
        existingTransaction.CategoryId = newTransaction.CategoryId;
        existingTransaction.TransactionSort = newTransaction.TransactionSort;

        // Update wallet-related fields only if they differ
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