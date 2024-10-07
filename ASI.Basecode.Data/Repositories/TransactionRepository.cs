// File: ASI.Basecode.Data/Repositories/TransactionRepository.cs

using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Linq; // Import for LINQ methods

namespace ASI.Basecode.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AsiBasecodeDbContext _context; // Use AsiBasecodeDbContext

        public TransactionRepository(AsiBasecodeDbContext context) // Constructor uses AsiBasecodeDbContext
        {
            _context = context;
        }

        public IEnumerable<MTransaction> RetrieveAll()
        {
            return _context.MTransactions.ToList(); // Retrieve all transactions
        }

        public MTransaction GetTransactionById(int id)
        {
            return _context.MTransactions.Find(id); // Get a transaction by its ID
        }

        public void Add(MTransaction transaction)
        {
            _context.MTransactions.Add(transaction);
            _context.SaveChanges();
        }

        public void Update(MTransaction transaction)
        {
            _context.MTransactions.Update(transaction);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var transaction = _context.MTransactions.Find(id);
            if (transaction != null)
            {
                _context.MTransactions.Remove(transaction);
                _context.SaveChanges();
            }
        }
    }
}
