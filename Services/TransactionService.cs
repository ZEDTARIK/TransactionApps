using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TransactionsApps.Models;

namespace TransactionsApps.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly TransactionDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TransactionService> _logger;

        private const string CacheKey_All = "Transactions:All";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30); // ← TTL utile

        public TransactionService(TransactionDbContext context, IMemoryCache cache, ILogger<TransactionService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            // GetOrCreateAsync simplifie la lecture + mise en cache
            return await _cache.GetOrCreateAsync(CacheKey_All, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheTtl;
                entry.SetPriority(CacheItemPriority.Normal);
                // entry.RegisterPostEvictionCallback((key, value, reason, state) => { /* log si besoin */ });

                return await _context.Transactions
                    .AsNoTracking()
                    .OrderByDescending(t => t.DateCreation) // tri souvent plus pertinent
                    .ToListAsync();
            }) ?? new List<Transaction>();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            // Lecture pure → AsNoTracking
            return await _context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TransactionId == id);
        }

        public async Task AddOrUpdateTransactionAsync(Transaction transaction)
        {
            try
            {
                if (transaction.TransactionId == 0)
                {
                    if (transaction.DateCreation == default)
                        transaction.DateCreation = DateTime.UtcNow; // UTC recommandé
                    await _context.Transactions.AddAsync(transaction);
                }
                else
                {
                    // Option 1 : mise à jour “attach + Modified” (simple et sûre)
                    _context.Attach(transaction);
                    _context.Entry(transaction).State = EntityState.Modified;

                    // Option 2 (ton approche) :
                    // var existing = await _context.Transactions.FindAsync(transaction.TransactionId);
                    // if (existing is null) throw new KeyNotFoundException("Transaction introuvable.");
                    // _context.Entry(existing).CurrentValues.SetValues(transaction);
                }

                await _context.SaveChangesAsync();

                InvalidateCache(); // ← invalider après écriture
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Conflit de concurrence sur Transaction {Id}", transaction.TransactionId);
                throw; // ou re-lancer une exception métier
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erreur de mise à jour SQL (validation schéma/contraintes).");
                throw; // ex: string truncation si SWIFT trop long
            }
        }

        public async Task DeleteTransactionAsync(int id)
        {
            // Suppression sans round-trip : stub
            var stub = new Transaction { TransactionId = id };
            _context.Transactions.Attach(stub);
            _context.Transactions.Remove(stub);

            try
            {
                await _context.SaveChangesAsync();
                InvalidateCache();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si l’ID n’existe plus, 0 lignes affectées → on peut ignorer ou remonter un 404 métier
                throw new KeyNotFoundException("Transaction introuvable.");
            }
        }

        private void InvalidateCache()
        {
            _cache.Remove(CacheKey_All);
        }
    }
}
