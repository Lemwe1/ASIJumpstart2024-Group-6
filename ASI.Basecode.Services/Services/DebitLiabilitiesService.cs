using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class DebitLiabilitiesService
    {
        private readonly AsiBasecodeDbContext _context;

        public DebitLiabilitiesService(AsiBasecodeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get all debit liabilities
        public async Task<List<MDebitLiab>> GetDebitLiabilitiesAsync()
        {
            return await _context.MDebitLiabs.ToListAsync();
        }

        // Add a new debit liability
        public async Task AddDebitLiabilityAsync(MDebitLiab model)
        {
            _context.MDebitLiabs.Add(model);
            await _context.SaveChangesAsync();
        }

        // Update an existing debit liability
        public async Task UpdateDebitLiabilityAsync(MDebitLiab model)
        {
            _context.MDebitLiabs.Update(model);
            await _context.SaveChangesAsync();
        }

        // Delete a debit liability
        public async Task DeleteDebitLiabilityAsync(int id)
        {
            var entity = await _context.MDebitLiabs.FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Debit Liability not found.");

            _context.MDebitLiabs.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
