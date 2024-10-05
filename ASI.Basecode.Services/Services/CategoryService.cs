using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore; // Ensure this is included
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Services
{
    public class CategoryService
    {
        private readonly AsiBasecodeDbContext _context;

        public CategoryService(AsiBasecodeDbContext context)
        {
            _context = context;
        }

        public async Task<List<MCategory>> GetCategoriesAsync()
        {
            return await _context.MCategories.ToListAsync(); // ToListAsync requires Microsoft.EntityFrameworkCore
        }

        public async Task AddCategoryAsync(MCategory category)
        {
            _context.MCategories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<MCategory> GetCategoryByIdAsync(int id)
        {
            return await _context.MCategories.AsNoTracking() // AsNoTracking requires Microsoft.EntityFrameworkCore
                                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task UpdateCategoryAsync(MCategory category)
        {
            var existingCategory = await _context.MCategories.FindAsync(category.CategoryId);
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
            var category = await _context.MCategories.FindAsync(id);
            if (category != null)
            {
                _context.MCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
        }
    }
}
