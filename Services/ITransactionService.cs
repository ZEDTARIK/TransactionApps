using TransactionsApps.helper;
using TransactionsApps.Models;

namespace TransactionsApps.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAllTransactionsAsync();
        Task<PaginatedList<Transaction>> GetTransactionsAsync(int pageNumber, int pageSize);
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task AddOrUpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(int id);
    }

}
