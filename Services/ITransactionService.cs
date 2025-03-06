using TransactionsApps.Models;

namespace TransactionsApps.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAllTransactionsAsync();
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task AddOrUpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(int id);
    }

}
