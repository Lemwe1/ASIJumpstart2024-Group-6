using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly List<MCategory> _categories = new List<MCategory>();
        private int _nextId = 1;

        // Get all categories (async)
        public async Task<IEnumerable<MCategory>> GetCategoriesAsync()
        {
            // Using Task.Run to simulate asynchronous behavior for in-memory data
            return await Task.Run(() => _categories.AsEnumerable());
        }

        // Get category by Id (async)
        public async Task<MCategory> GetCategoryByIdAsync(int id)
        {
            return await Task.Run(() => _categories.FirstOrDefault(c => c.CategoryId == id));
        }

        // Add a new category (async)
        public async Task AddCategoryAsync(MCategory category)
        {
            await Task.Run(() =>
            {
                category.CategoryId = _nextId++;
                _categories.Add(category);
            });
        }

        // Update an existing category (async)
        public async Task UpdateCategoryAsync(MCategory category)
        {
            await Task.Run(() =>
            {
                var existingCategory = _categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                if (existingCategory != null)
                {
                    existingCategory.Name = category.Name;
                    existingCategory.Icon = category.Icon;
                    existingCategory.Color = category.Color;
                    existingCategory.Type = category.Type;
                }
            });
        }

        // Delete a category by Id (async)
        public async Task DeleteCategoryAsync(int id)
        {
            await Task.Run(() =>
            {
                var category = _categories.FirstOrDefault(c => c.CategoryId == id);
                if (category != null)
                {
                    _categories.Remove(category);
                }
            });
        }
    }
}
