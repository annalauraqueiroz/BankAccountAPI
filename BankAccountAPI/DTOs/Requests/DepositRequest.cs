namespace BankingSystem.DTOs.Requests
{
    public class DepositRequest
    {
        public int AccountId { get; set; }
        public long Amount { get; set; }
        public DepositRequest(int accountId, long amount)
        {
            AccountId = accountId;
            Amount = amount;
        }
    }
}
