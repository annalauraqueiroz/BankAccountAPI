using BankingSystem.Data;
using BankingSystem.Domain.Entities;
using BankingSystem.Repositories.Interfaces;

namespace BankingSystem.Repositories
{
    public class TransactionRepository(BankDbContext context) : ITransactionRepository
    {
        private readonly BankDbContext _context = context;

        public async Task AddTransactionsAsync(List<Transaction> transactions)
        {
            _context.Transactions.AddRange(transactions);
            await _context.SaveChangesAsync();
        }

    }
}
