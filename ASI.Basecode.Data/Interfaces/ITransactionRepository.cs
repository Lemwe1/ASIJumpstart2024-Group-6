// File: ASI.Basecode.Data/Interfaces/ITransactionRepository.cs

using ASI.Basecode.Data.Models;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ITransactionRepository
    {
        IEnumerable<MTransaction> RetrieveAll();
        MTransaction GetTransactionById(int id);

        void Add(MTransaction transaction);
        void Update(MTransaction transaction);
        void Delete(int id);
    }
}
