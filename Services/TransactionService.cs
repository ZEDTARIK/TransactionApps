using Microsoft.EntityFrameworkCore;
using TransactionsApps.Models;

namespace TransactionsApps.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly TransactionDbContext _context;

        public TransactionService(TransactionDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions.AsNoTracking().ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task AddOrUpdateTransactionAsync(Transaction transaction)
        {
            if (transaction.TransactionId == 0)
            {
                transaction.DateCreation = DateTime.Now;
                _context.Add(transaction);
            }
            else
            {
                _context.Update(transaction);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
    }

}
