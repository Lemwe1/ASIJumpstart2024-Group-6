﻿using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AsiBasecodeDbContext _context;
        private readonly ITransactionRepository _transactionRepository;

        public CategoryService(AsiBasecodeDbContext context, ITransactionRepository transactionRepository)
        {
            _context = context;
            _transactionRepository = transactionRepository;
        }

        // Get both user-specific and global categories
        public async Task<List<MCategory>> GetCategoriesAsync(int? userId)
        {
            return await _context.MCategories
                       .Where(c => c.UserId == userId || c.IsGlobal == true)
                       .ToListAsync();
        }

        // Add a new category for a specific user
        public async Task AddCategoryAsync(MCategory category, int? userId)
        {
            // **Modification Starts Here**

            // Check if category with the same name already exists for the user or globally
            bool categoryExists = await _context.MCategories
                .AnyAsync(c => c.Name == category.Name && (c.UserId == userId || c.IsGlobal));

            if (categoryExists)
            {
                throw new InvalidOperationException("A category with the same name already exists.");
            }

            // **Modification Ends Here**

            category.UserId = userId; // Assign the category to the user
            _context.MCategories.Add(category);
            await _context.SaveChangesAsync();
        }

        // Get category by Id, accommodating both user-specific and global categories
        public async Task<MCategory> GetCategoryByIdAsync(int id, int? userId)
        {
            return await _context.MCategories
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.CategoryId == id && (c.UserId == userId || c.IsGlobal == true));
        }

        // Update a category for a specific user, accommodating global categories if needed
        public async Task UpdateCategoryAsync(MCategory category, int? userId)
        {
            var existingCategory = await _context.MCategories
                                                 .FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId && (c.UserId == userId || c.IsGlobal == true));
            if (existingCategory != null)
            {
                // **Modification Starts Here**

                // Check if another category with the same name exists (excluding the current category)
                bool categoryExists = await _context.MCategories
                    .AnyAsync(c => c.Name == category.Name && c.CategoryId != category.CategoryId && (c.UserId == userId || c.IsGlobal));

                if (categoryExists)
                {
                    throw new InvalidOperationException("A category with the same name already exists.");
                }

                // **Modification Ends Here**

                // Update fields for both user-specific and global categories
                existingCategory.Name = category.Name;
                existingCategory.Type = category.Type;
                existingCategory.Icon = category.Icon;
                existingCategory.Color = category.Color;

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Category with ID {category.CategoryId} not found for user {userId}.");
            }
        }

        // Delete a category by Id, with restrictions on specific global categories
        public async Task DeleteCategoryAsync(int id, int? userId)
        {
            var category = await _context.MCategories
                                         .FirstOrDefaultAsync(c => c.CategoryId == id && (c.UserId == userId || c.IsGlobal == true));

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found for user {userId}.");
            }

            // Prevent deletion of specific global categories: "Default Income" and "Default Expense"
            if (category.IsGlobal && (category.Name == "Default Income" || category.Name == "Default Expense"))
            {
                throw new InvalidOperationException("Default categories cannot be deleted.");
            }

            // Check if the category is in use in any transactions
            bool isCategoryInUse = await _transactionRepository.IsCategoryInUseAsync(id);
            if (isCategoryInUse)
            {
                throw new InvalidOperationException("Category is in use in Transactions and cannot be deleted.");
            }

            // If not in use, proceed with deletion
            _context.MCategories.Remove(category);
            await _context.SaveChangesAsync();
        }

        // Get category name by ID, accommodating both user-specific and global categories
        public async Task<string> GetCategoryNameByIdAsync(int categoryId, int? userId)
        {
            var category = await _context.MCategories
                                         .FirstOrDefaultAsync(c => c.CategoryId == categoryId && (c.UserId == userId || c.IsGlobal == true));
            return category?.Name; // Return the category name or null if not found
        }
    }
}
