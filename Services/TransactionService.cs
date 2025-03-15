using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TransactionsApps.helper;
using TransactionsApps.Models;

namespace TransactionsApps.Services
{
    public class TransactionService(TransactionDbContext context, IMemoryCache cache) : ITransactionService
    {
        private readonly TransactionDbContext _context = context;
        private readonly IMemoryCache _cache = cache;
        private readonly string _cacheKey = "Transactions";
        private readonly TimeSpan timeSpan = TimeSpan.FromMilliseconds(300);

        public async Task<PaginatedList<Transaction>> GetTransactionsAsync(int pageNumber, int pageSize)
        {
            var query = _context.Transactions
                    .AsNoTracking().OrderByDescending(t => t.TransactionId);
            return await PaginatedList<Transaction>.CreateAsync(query, pageNumber, pageSize);
        }


        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            if (!_cache.TryGetValue(_cacheKey, out List<Transaction>? transactions))
            {
                transactions = await _context.Transactions
                        .AsNoTracking().OrderByDescending(t => t.TransactionId).ToListAsync();

                // Ajouter les transactions en cache
                _cache.Set(_cacheKey, transactions, timeSpan);
            }

            return transactions ?? [];
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
