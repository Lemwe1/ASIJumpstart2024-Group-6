using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IWalletService
    {
        Task<IEnumerable<WalletViewModel>> GetWalletAsync(int userId);
        Task<WalletViewModel> GetByIdAsync(int id);
        Task AddWalletAsync(WalletViewModel model);
        Task UpdateWalletAsync(WalletViewModel model);
        Task DeleteWalletAsync(int id);
        Task<string> GetWalletNameByIdAsync(int walletId, int userId); // Add this method
    }
}