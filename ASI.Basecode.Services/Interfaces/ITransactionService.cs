using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ITransactionService
    {
        IEnumerable<MTransaction> GetAllTransactions();
        TransactionViewModel GetTransactionById(int id);
        void CreateTransaction(TransactionViewModel transactionViewModel);
        void UpdateTransaction(TransactionViewModel transactionViewModel);
        void DeleteTransaction(int id);
    }
}
