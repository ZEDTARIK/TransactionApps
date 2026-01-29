using Microsoft.EntityFrameworkCore;

namespace TransactionsApps.Models
{
    public class TransactionDbContext(DbContextOptions<TransactionDbContext> options) : DbContext(options)
    {
        public DbSet<Transaction> Transactions { get; set; }
    }

}
