namespace BankingSystem.DTOs.Requests
{
    public class WithdrawRequest
    {
        public int AccountId { get; set; }
        public long Amount { get; set; }
        public WithdrawRequest(int accountId, long amount)
        {
            AccountId = accountId;
            Amount = amount;
        }
    }
}
