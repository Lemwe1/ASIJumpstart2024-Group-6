using System.Collections.Generic;
using System.Threading.Tasks;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionViewModel>> GetAllTransactionsAsync(int userId);
        Task<TransactionViewModel> GetTransactionByIdAsync(int model);
        Task AddTransactionAsync(TransactionViewModel model);
        Task UpdateTransactionAsync(TransactionViewModel model);
        Task DeleteTransactionAsync(int id);
        Task UpdateBudgetAfterTransaction(int categoryId, int userId);

    }
}
