using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IWalletRepository
    {
        Task<IEnumerable<MWallet>> RetrieveAllAsync(); 
        Task<MWallet> GetByIdAsync(int id);
        Task AddAsync(MWallet model);
        Task UpdateAsync(MWallet model);
        Task DeleteAsync(int id);
        Task<IEnumerable<MWallet>> GetAllAsync(); 
    }
}
