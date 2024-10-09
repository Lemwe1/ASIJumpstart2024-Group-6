using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IDebitLiabilitiesService
    {
        Task<IEnumerable<DebitLiabilityViewModel>> GetDebitLiabilitiesAsync(int userId);
        Task<DebitLiabilityViewModel> GetByIdAsync(int id);
        Task AddDebitLiabilityAsync(DebitLiabilityViewModel model);
        Task UpdateDebitLiabilityAsync(DebitLiabilityViewModel model);
        Task DeleteDebitLiabilityAsync(int id);
    }
}
