using ASI.Basecode.Data.Models;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ICategoryRepository
    {
        IEnumerable<MCategory> GetCategoriesAsync();
        MCategory GetCategoryByIdAsync(int id);
        void AddCategoryAsync(MCategory category);
        void UpdateCategoryAsync(MCategory category);
        void DeleteCategoryAsync(int id);
    }
}
