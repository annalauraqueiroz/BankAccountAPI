namespace BankingSystem.DTOs.Requests
{
    public class TransferRequest
    {
        public int SourceAccountId { get; set; }
        public int DestinationAccountId { get; set; }
        public long Amount { get; set; }
    }
}
