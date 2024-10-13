using System.Collections.Generic;
using System.Threading.Tasks;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionViewModel>> GetAllTransactionsAsync(int userId);
        Task<TransactionViewModel> GetTransactionByIdAsync(int transactionId);
        Task AddTransactionAsync(TransactionViewModel transactionViewModel);
        Task UpdateTransactionAsync(TransactionViewModel transactionViewModel);
        Task DeleteTransactionAsync(int transactionId);
    }
}
