using System.Collections.Generic;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<MTransaction>> GetAllAsync();
        Task<MTransaction> GetByIdAsync(int transactionId);
        Task AddAsync(MTransaction transaction);
        Task UpdateAsync(MTransaction transaction);
        Task DeleteAsync(int transactionId);
        Task<int> CountCategoriesAsync();
        Task<decimal> GetTotalExpenseAmountAsync();
    }
}
