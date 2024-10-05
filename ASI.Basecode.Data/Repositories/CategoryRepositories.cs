using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly List<MCategory> _categories = new List<MCategory>();
        private int _nextId = 1;

        public IEnumerable<MCategory> GetCategoriesAsync()
        {
            return _categories;
        }

        public MCategory GetCategoryByIdAsync(int id)
        {
            return _categories.FirstOrDefault(c => c.CategoryId == id);
        }

        public void AddCategoryAsync(MCategory category)
        {
            category.CategoryId = _nextId++;
            _categories.Add(category);
        }

        public void UpdateCategoryAsync(MCategory category)
        {
            var existingCategory = _categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.Icon = category.Icon;
                existingCategory.Color = category.Color;
                existingCategory.Type = category.Type;
            }
        }

        public void DeleteCategoryAsync(int id)
        {
            var category = _categories.FirstOrDefault(c => c.CategoryId == id);
            if (category != null)
            {
                _categories.Remove(category);
            }
        }
    }
}
