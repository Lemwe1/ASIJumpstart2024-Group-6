using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class DebitLiabilitiesService : IDebitLiabilitiesService
    {
        private readonly IDebitLiabilitiesRepository _debitLiabilitiesRepository;

        public DebitLiabilitiesService(IDebitLiabilitiesRepository debitLiabilitiesRepository)
        {
            _debitLiabilitiesRepository = debitLiabilitiesRepository;
        }

        public async Task<IEnumerable<DebitLiabilityViewModel>> GetDebitLiabilitiesAsync(int userId)
        {
            var allLiabilities = await _debitLiabilitiesRepository.RetrieveAllAsync();
            return allLiabilities
                .Where(x => x.UserId == userId)
                .Select(MapToViewModel);
        }

        public async Task<DebitLiabilityViewModel> GetByIdAsync(int id)
        {
            var model = await _debitLiabilitiesRepository.GetByIdAsync(id);
            return model != null ? MapToViewModel(model) : null;
        }

        public async Task AddDebitLiabilityAsync(DebitLiabilityViewModel viewModel)
        {
            var model = MapToModel(viewModel);
            await _debitLiabilitiesRepository.AddAsync(model);
        }

        public async Task UpdateDebitLiabilityAsync(DebitLiabilityViewModel viewModel)
        {
            var model = MapToModel(viewModel);
            await _debitLiabilitiesRepository.UpdateAsync(model);
        }

        public async Task DeleteDebitLiabilityAsync(int id)
        {
            await _debitLiabilitiesRepository.DeleteAsync(id);
        }

        // Mapping from MDebitLiab to DebitLiabilityViewModel
        private DebitLiabilityViewModel MapToViewModel(MDebitLiab model)
        {
            return new DebitLiabilityViewModel
            {
                DeLiId = model.DeLiId,
                DeLiType = model.DeLiType,
                DeLiBalance = model.DeLiBalance,
                DeLiIcon = model.DeLiIcon,
                DeLiColor = model.DeLiColor,
                DeLiHapp = model.DeLiHapp,
                DeLiDue = model.DeLiDue,
                DeLiName = model.DeLiName,
                UserId = model.UserId
            };
        }

        // Mapping from DebitLiabilityViewModel to MDebitLiab
        private MDebitLiab MapToModel(DebitLiabilityViewModel viewModel)
        {
            return new MDebitLiab
            {
                DeLiId = viewModel.DeLiId,
                DeLiType = viewModel.DeLiType,
                DeLiBalance = viewModel.DeLiBalance,
                DeLiIcon = viewModel.DeLiIcon,
                DeLiColor = viewModel.DeLiColor,
                DeLiHapp = viewModel.DeLiHapp,
                DeLiDue = viewModel.DeLiDue,
                DeLiName = viewModel.DeLiName,
                UserId = viewModel.UserId
            };
        }
    }
}
