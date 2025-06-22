using BankingSystem.Domain.Constants;
using BankingSystem.Domain.Entities;
using BankingSystem.Domain.Enums;
using BankingSystem.DTOs.Requests;
using BankingSystem.DTOs.Responses;
using BankingSystem.Repositories.Interfaces;
using BankingSystem.Services.Interfaces;

namespace BankingSystem.Services
{
    public class AccountService(IAccountRepository accountRepository, ITransactionRepository transactionRepository) : IAccountService
    {
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly ITransactionRepository _transactionRepository = transactionRepository;

        public async Task<AccountResponse?> GetAccountAsync(int accountId)
        {
            return await _accountRepository.GetAccountAsync(accountId);
        }

        public async Task<List<AccountResponse>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAccountsAsync();
        }
        public async Task<AccountResponse> CreateAccountAsync(AccountRequest accountRequest)
        {
            return await _accountRepository.CreateAccountAsync(accountRequest.AccountHolderName);
        }
        public async Task<bool?> UpdateAccountAsync(int accountId, AccountRequest accountRequest)
        {
            var account = await _accountRepository.GetAccountModelAsync(accountId);

            if (account is null)
                return null;

            account.AccountHolderName = accountRequest.AccountHolderName;

            await _accountRepository.UpdateAccountAsync(account);

            return true;
        }
        public async Task<bool?> DeleteAccountAsync(int accountId)
        {
            var account = await _accountRepository.GetAccountModelAsync(accountId);

            if (account is null)
                return false;

            if (account.CurrentBalance > 0)
                return false;

            await _accountRepository.DeleteAccountAsync(account);

            return true;
        }
        public async Task<List<TransactionResponse>?> GetStatementAsync(int accountId)
        {
            return await _accountRepository.GetAccountTransactionsByAccountIdAsync(accountId);
        }
        public async Task<bool> DepositAsync(DepositRequest depositRequest)
        {
            List<Transaction> transactions = new List<Transaction>();
            var account = await _accountRepository.GetAccountModelAsync(depositRequest.AccountId);

            if (account is null) return false;

            transactions.Add(new Transaction(
                depositRequest.AccountId,
                depositRequest.Amount,
                "Deposit",
                TransactionType.Credit,
                TransactionCategory.Deposit
            ));

            transactions.Add(new Transaction(
                    depositRequest.AccountId,
                    (long)Math.Ceiling(depositRequest.Amount * TransactionFees.DepositPercentage),
                    "Deposit fee",
                    TransactionType.Debit,
                    TransactionCategory.Fee
            ));

            await _transactionRepository.AddTransactionsAsync(transactions);

            return true;
        }

        public async Task<bool> WithdrawAsync(WithdrawRequest withdrawRequest)
        {
            List<Transaction> transactions = new List<Transaction>();
            var account = await _accountRepository.GetAccountModelAsync(withdrawRequest.AccountId);

            if (account is null) return false;

            long totalAmount = withdrawRequest.Amount + TransactionFees.WithdrawFixed;

            if (account.CurrentBalance < totalAmount) return false;

            transactions.Add(new Transaction(
                withdrawRequest.AccountId,
                withdrawRequest.Amount,
                "Withdraw",
                TransactionType.Debit,
                TransactionCategory.Withdraw
            ));

            transactions.Add(new Transaction(
                    withdrawRequest.AccountId,
                    TransactionFees.WithdrawFixed,
                    $"Withdraw fee",
                    TransactionType.Debit,
                    TransactionCategory.Fee
            ));

            await _transactionRepository.AddTransactionsAsync(transactions);

            return true;
        }

        public async Task<bool> TransferAsync(TransferRequest transferRequest)
        {
            var sourceAccountTask = _accountRepository.GetAccountModelAsync(transferRequest.SourceAccountId);
            var destinationAccountTask = _accountRepository.GetAccountModelAsync(transferRequest.DestinationAccountId);

            await Task.WhenAll(sourceAccountTask, destinationAccountTask);

            Account? sourceAccount = await sourceAccountTask;
            Account? destinationAccount = await destinationAccountTask;

            if (sourceAccount is null || destinationAccount is null) return false;

            long totalAmount = transferRequest.Amount + TransactionFees.TransferFixed;

            if (sourceAccount.CurrentBalance < totalAmount) return false;

            List<Transaction> transactions =
            [
                new Transaction(
                    sourceAccount.AccountId,
                    transferRequest.Amount,
                    $"Transfer to {destinationAccount.AccountHolderName}",
                    TransactionType.Debit,
                    TransactionCategory.Transfer
                ),
                new Transaction(
                    sourceAccount.AccountId,
                    TransactionFees.TransferFixed,
                    $"Transfer fee",
                    TransactionType.Debit,
                    TransactionCategory.Fee
                ),
                new Transaction(
                    destinationAccount.AccountId,
                    transferRequest.Amount,
                    $"Transfer from {sourceAccount.AccountHolderName}",
                    TransactionType.Credit,
                    TransactionCategory.Transfer
                )
            ];

            await _transactionRepository.AddTransactionsAsync(transactions);

            return true;
        }


    }
}
