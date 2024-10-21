using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using ASI.Basecode.WebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AsiBasecodeDbContext _context;

        public CategoryService(AsiBasecodeDbContext context)
        {
            _context = context;
        }


        public async Task<List<MCategory>> GetCategoriesAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid userId");
            }

            return await _context.MCategories
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        // Add a new category for a specific user
        public async Task AddCategoryAsync(MCategory category, string userId)
        {
            if (!int.TryParse(userId, out int userIdInt))
            {
                throw new ArgumentException("Invalid user ID");
            }

            category.UserId = userIdInt; // Assign the category to the user
            _context.MCategories.Add(category);
            await _context.SaveChangesAsync();
        }

        // Get category by Id and UserId
        public async Task<MCategory> GetCategoryByIdAsync(int id, string userId)
        {
            if (!int.TryParse(userId, out int userIdInt))
            {
                throw new ArgumentException("Invalid user ID");
            }

            return await _context.MCategories
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userIdInt);
        }

        // Update a category for a specific user
        public async Task UpdateCategoryAsync(MCategory category, string userId)
        {
            if (!int.TryParse(userId, out int userIdInt))
            {
                throw new ArgumentException("Invalid user ID");
            }

            var existingCategory = await _context.MCategories
                                                 .FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId && c.UserId == userIdInt);
            if (existingCategory != null)
            {
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

        // Delete a category by Id and UserId
        public async Task DeleteCategoryAsync(int id, string userId)
        {
            if (!int.TryParse(userId, out int userIdInt))
            {
                throw new ArgumentException("Invalid user ID");
            }

            var category = await _context.MCategories
                                         .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userIdInt);
            if (category != null)
            {
                _context.MCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Category with ID {id} not found for user {userId}.");
            }
        }

        // Get category name by ID
        public async Task<string> GetCategoryNameByIdAsync(int categoryId, int userId)
        {
            var category = await _context.MCategories
                                         .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.UserId == userId);
            return category?.Name; // Return the category name or null if not found
        }
    }
}