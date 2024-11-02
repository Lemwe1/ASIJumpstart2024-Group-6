﻿using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
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
            await _walletRepository.AddAsync(model);
        }

        public async Task UpdateWalletAsync(WalletViewModel viewModel)
        {
            var model = MapToModel(viewModel);
            await _walletRepository.UpdateAsync(model);
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
    }
}