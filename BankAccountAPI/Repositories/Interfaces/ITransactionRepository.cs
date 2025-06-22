using BankingSystem.Domain.Entities;

namespace BankingSystem.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddTransactionsAsync(List<Transaction> transactions);
    }
}
