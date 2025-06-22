using BankingSystem.Data;
using BankingSystem.Domain.Entities;
using BankingSystem.DTOs.Responses;
using BankingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Repositories
{
    public class AccountRepository(BankDbContext context) : IAccountRepository
    {
        private readonly BankDbContext _context = context;

        public async Task<AccountResponse?> GetAccountAsync(int accountId)
        {
            return await _context.Accounts
                .Include(a => a.Transactions)
                .Select(a => new AccountResponse
                {
                    AccountHolderName = a.AccountHolderName,
                    AccountId = a.AccountId,
                    CurrentBalance = a.CurrentBalance
                })
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }
        public async Task<Account?> GetAccountModelAsync(int accountId)
        {
            return await _context.Accounts.Include(a => a.Transactions).FirstOrDefaultAsync(a => a.AccountId == accountId);
        }
        public async Task<List<AccountResponse>> GetAllAccountsAsync()
        {
            return await _context.Accounts
            .Include(a => a.Transactions)
            .Select(a => new AccountResponse
            {
                AccountHolderName = a.AccountHolderName,
                AccountId = a.AccountId,
                CurrentBalance = a.CurrentBalance
            })
            .ToListAsync();
        }

        public async Task<AccountResponse> CreateAccountAsync(string holderName)
        {
            var account = new Account() { AccountHolderName = holderName };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return new AccountResponse
            {
                AccountHolderName = account.AccountHolderName,
                AccountId = account.AccountId,
                CurrentBalance = account.CurrentBalance
            };
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(Account account)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
        }
        public async Task<List<TransactionResponse>?> GetAccountTransactionsByAccountIdAsync(int accountId)
        {
            var account = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            return account?.Transactions
                                .Select(t => new TransactionResponse
                                {
                                    Date = t.Date,
                                    AccountId = t.AccountId,
                                    Amount = t.Amount,
                                    Description = t.Description,
                                    Type = t.Type,
                                    Category = t.Category
                                })
                                .OrderByDescending(t => t.Date)
                                .ToList();
        }
    }
}
