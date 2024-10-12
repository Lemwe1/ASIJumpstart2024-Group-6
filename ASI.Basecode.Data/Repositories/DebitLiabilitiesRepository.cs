using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class DebitLiabilitiesRepository : IDebitLiabilitiesRepository
    {
        private readonly AsiBasecodeDbContext _context;

        public DebitLiabilitiesRepository(AsiBasecodeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MDebitLiab>> RetrieveAllAsync()
        {
            return await _context.MDebitLiabs.ToListAsync();
        }

        public async Task<MDebitLiab> GetByIdAsync(int id)
        {
            return await _context.MDebitLiabs.FindAsync(id);
        }

        public async Task AddAsync(MDebitLiab model)
        {
            await _context.MDebitLiabs.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MDebitLiab model)
        {
            _context.MDebitLiabs.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var model = await GetByIdAsync(id);
            if (model != null)
            {
                _context.MDebitLiabs.Remove(model);
                await _context.SaveChangesAsync();
            }
        }

        // Implement the GetAllAsync method
        public async Task<IEnumerable<MDebitLiab>> GetAllAsync()
        {
            return await _context.MDebitLiabs.ToListAsync(); // Example implementation
        }
    }
}
