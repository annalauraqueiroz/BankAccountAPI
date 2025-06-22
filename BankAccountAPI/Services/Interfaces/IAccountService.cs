using BankingSystem.DTOs.Requests;
using BankingSystem.DTOs.Responses;

namespace BankingSystem.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponse?> GetAccountAsync(int accountId);
        Task<List<AccountResponse>> GetAllAccountsAsync();
        Task<AccountResponse> CreateAccountAsync(AccountRequest accountRequest);
        Task<bool?> UpdateAccountAsync(int accountId, AccountRequest request);
        Task<bool?> DeleteAccountAsync(int accountId);
        Task<List<TransactionResponse>?> GetStatementAsync(int accountId);
        Task<bool> DepositAsync(DepositRequest request);
        Task<bool> WithdrawAsync(WithdrawRequest request);
        Task<bool> TransferAsync(TransferRequest request);
    }
}
