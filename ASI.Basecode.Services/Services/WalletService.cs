using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<IEnumerable<WalletViewModel>> GetWalletAsync(int userId)
        {
            var allLiabilities = await _walletRepository.RetrieveAllAsync();
            return allLiabilities
                .Where(x => x.UserId == userId)
                .Select(MapToViewModel);
        }

        public async Task<WalletViewModel> GetByIdAsync(int id)
        {
            var model = await _walletRepository.GetByIdAsync(id);
            return model != null ? MapToViewModel(model) : null;
        }

        public async Task AddWalletAsync(WalletViewModel viewModel)
        {
            var model = MapToModel(viewModel);

            // Set WalletOriginalBalance when adding a new wallet
            model.WalletOriginalBalance = model.WalletBalance; 

            await _walletRepository.AddAsync(model);
        }

        public async Task UpdateWalletAsync(WalletViewModel viewModel)
        {
            // Retrieve the wallet from the repository
            var wallet = await _walletRepository.GetByIdAsync(viewModel.WalletId);

            if (wallet == null)
            {
                throw new Exception($"Wallet with ID {viewModel.WalletId} not found.");
            }

            // Track the original wallet balance if not already set
            if (wallet.WalletOriginalBalance == 0)
            {
                wallet.WalletOriginalBalance = wallet.WalletBalance; // Set the original balance only once
            }

            // Update the current wallet balance
            wallet.WalletBalance = viewModel.WalletBalance;
            wallet.WalletName = viewModel.WalletName;
            wallet.WalletIcon = viewModel.WalletIcon;
            wallet.WalletColor = viewModel.WalletColor;

            // Optionally log or handle the difference between the original and updated balances
            var balanceChange = wallet.WalletBalance - wallet.WalletOriginalBalance;

            // Save the updated wallet to the repository
            await _walletRepository.UpdateAsync(wallet);
        }



        public async Task DeleteWalletAsync(int id)
        {
            await _walletRepository.DeleteAsync(id);
        }

        // Mapping from MWallet to WalletViewModel
        private WalletViewModel MapToViewModel(MWallet model)
        {
            return new WalletViewModel
            {
                WalletId = model.WalletId,
                WalletBalance = model.WalletBalance,
                WalletOriginalBalance = model.WalletOriginalBalance,
                WalletIcon = model.WalletIcon,
                WalletColor = model.WalletColor,
                WalletName = model.WalletName,
                UserId = model.UserId
            };
        }

        // Mapping from WalletViewModel to MWallet
        private MWallet MapToModel(WalletViewModel viewModel)
        {
            return new MWallet
            {
                WalletId = viewModel.WalletId,
                WalletBalance = viewModel.WalletBalance,
                WalletOriginalBalance = viewModel.WalletOriginalBalance,
                WalletIcon = viewModel.WalletIcon,
                WalletColor = viewModel.WalletColor,
                WalletName = viewModel.WalletName,
                UserId = viewModel.UserId
            };
        }

        // Get debit/liability name by ID
        public async Task<string> GetWalletNameByIdAsync(int walletId, int userId)
        {
            var debitLiability = await _walletRepository.GetByIdAsync(walletId);
            return debitLiability?.WalletName; // Return the category name or null if not found
        }
        public async Task<bool> WalletExistsAsync(int userId, string walletName)
        {
            var debitLiability = await _walletRepository.RetrieveAllAsync();
            return debitLiability.Any(x => x.UserId == userId && x.WalletName.Equals(walletName, StringComparison.OrdinalIgnoreCase));
        }
    }
}