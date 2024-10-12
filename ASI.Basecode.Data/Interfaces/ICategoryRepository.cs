using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<MCategory>> GetCategoriesAsync();
        Task <MCategory> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(MCategory category);
        Task UpdateCategoryAsync(MCategory category);
        Task DeleteCategoryAsync(int id);
    }
}
