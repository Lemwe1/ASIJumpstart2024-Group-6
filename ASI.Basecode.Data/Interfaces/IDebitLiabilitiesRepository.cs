using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IDebitLiabilitiesRepository
    {
        Task<IEnumerable<MDebitLiab>> RetrieveAllAsync(); 
        Task<MDebitLiab> GetByIdAsync(int id);
        Task AddAsync(MDebitLiab model);
        Task UpdateAsync(MDebitLiab model);
        Task DeleteAsync(int id);
        Task<IEnumerable<MDebitLiab>> GetAllAsync(); 
    }
}
