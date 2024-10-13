﻿using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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
        return transactions
            .Where(t => t.UserId == userId)
            .Select(MapToViewModel); 
    }

    // Get a specific transaction by ID
    public async Task<TransactionViewModel> GetTransactionByIdAsync(int transactionId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        return transaction != null ? MapToViewModel(transaction) : null;
    }

    // Add a new transaction
    public async Task AddTransactionAsync(TransactionViewModel transactionViewModel)
    {
        var transaction = MapToModel(transactionViewModel); 
        await _transactionRepository.AddAsync(transaction);
    }

    // Update an existing transaction
    public async Task UpdateTransactionAsync(TransactionViewModel transactionViewModel)
    {
        var transaction = MapToModel(transactionViewModel); 
        await _transactionRepository.UpdateAsync(transaction);
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
            Description = model.Description,
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
            Description = viewModel.Description,
            Amount = viewModel.Amount,
            TransactionDate = viewModel.TransactionDate,
            Note = viewModel.Note
        };
    }
}
