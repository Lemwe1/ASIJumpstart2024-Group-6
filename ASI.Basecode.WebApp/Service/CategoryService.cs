using ASI.Basecode.WebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ASI.Basecode.WebApp.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryModel>> GetCategoriesAsync()
        {
            return await _context.M_Category.ToListAsync();
        }

        public async Task AddCategoryAsync(CategoryModel category)
        {
            _context.M_Category.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<CategoryModel> GetCategoryByIdAsync(int id)
        {
            // Use AsNoTracking if fetching for display purposes only
            return await _context.M_Category.AsNoTracking().FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task UpdateCategoryAsync(CategoryModel category)
        {
            var existingCategory = await _context.M_Category.FindAsync(category.CategoryId);
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
                throw new KeyNotFoundException($"Category with ID {category.CategoryId} not found.");
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.M_Category.FindAsync(id);
            if (category != null)
            {
                _context.M_Category.Remove(category);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
        }
    }
}
