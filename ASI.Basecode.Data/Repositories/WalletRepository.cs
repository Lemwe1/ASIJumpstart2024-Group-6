using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AsiBasecodeDbContext _context;

        public WalletRepository(AsiBasecodeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MWallet>> RetrieveAllAsync()
        {
            return await _context.MWallets.ToListAsync();
        }

        public async Task<MWallet> GetByIdAsync(int id)
        {
            return await _context.MWallets.FindAsync(id);
        }

        public async Task AddAsync(MWallet model)
        {
            await _context.MWallets.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MWallet model)
        {
            _context.MWallets.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var model = await GetByIdAsync(id);
            if (model != null)
            {
                _context.MWallets.Remove(model);
                await _context.SaveChangesAsync();
            }
        }

        // Implement the GetAllAsync method
        public async Task<IEnumerable<MWallet>> GetAllAsync()
        {
            return await _context.MWallets.ToListAsync(); // Example implementation
        }
    }
}
