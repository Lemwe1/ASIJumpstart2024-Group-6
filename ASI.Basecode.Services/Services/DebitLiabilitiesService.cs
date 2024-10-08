using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class DebitLiabilitiesService
    {
        private readonly IDebitLiabilitiesRepository _debitLiabilitiesRepository;

        public DebitLiabilitiesService(IDebitLiabilitiesRepository debitLiabilitiesRepository)
        {
            _debitLiabilitiesRepository = debitLiabilitiesRepository;
        }

        public async Task<IEnumerable<MDebitLiab>> GetDebitLiabilitiesAsync(int userId)
        {
            // Await the task to get the result
            var allLiabilities = await _debitLiabilitiesRepository.RetrieveAllAsync();

            // Filter the results based on the userId
            return allLiabilities.Where(x => x.UserId == userId);
        }




        public async Task AddDebitLiabilityAsync(MDebitLiab model)
        {
            await _debitLiabilitiesRepository.AddAsync(model);
        }

        public async Task UpdateDebitLiabilityAsync(MDebitLiab model)
        {
            await _debitLiabilitiesRepository.UpdateAsync(model);
        }

        public async Task DeleteDebitLiabilityAsync(int id)
        {
            await _debitLiabilitiesRepository.DeleteAsync(id);
        }
    }
}
