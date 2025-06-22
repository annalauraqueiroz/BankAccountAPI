using BankingSystem.Domain.Entities;
using BankingSystem.DTOs.Responses;

namespace BankingSystem.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<AccountResponse?> GetAccountAsync(int accountId);
        Task<Account?> GetAccountModelAsync(int accountId);
        Task<AccountResponse> CreateAccountAsync(string holderName);
        Task<List<AccountResponse>> GetAllAccountsAsync();
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(Account account);
        Task<List<TransactionResponse>?> GetAccountTransactionsByAccountIdAsync(int accountId);
    }
}
