using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<MCategory>> GetCategoriesAsync(int? userId);
        Task AddCategoryAsync(MCategory category, int? userId);
        Task<MCategory> GetCategoryByIdAsync(int id, int? userId);
        Task UpdateCategoryAsync(MCategory category, int? userId);
        Task DeleteCategoryAsync(int id, int? userId);
        Task<string> GetCategoryNameByIdAsync(int categoryId, int? userId);
    }
}
