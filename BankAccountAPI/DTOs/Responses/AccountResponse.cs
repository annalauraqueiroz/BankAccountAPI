namespace BankingSystem.DTOs.Responses
{
    public class AccountResponse
    {
        public string AccountHolderName { get; set; }
        public int AccountId { get; set; }
        public long CurrentBalance { get; set; }
    }
}
