using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<MCategory>> GetCategoriesAsync(int userId);
        Task AddCategoryAsync(MCategory category, string userId);
        Task<MCategory> GetCategoryByIdAsync(int id, string userId);
        Task UpdateCategoryAsync(MCategory category, string userId);
        Task DeleteCategoryAsync(int id, string userId);
        Task<string> GetCategoryNameByIdAsync(int categoryId, int userId); // Add this method
    }
}