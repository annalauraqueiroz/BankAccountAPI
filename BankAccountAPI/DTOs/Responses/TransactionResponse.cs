using BankingSystem.Domain.Enums;

namespace BankingSystem.DTOs.Responses
{
    public class TransactionResponse
    {
        public DateTime Date { get; set; }
        public int AccountId { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }

        public TransactionType Type { get; set; }
        public string TypeDescription => Type.ToString();

        public TransactionCategory Category { get; set; }
        public string CategoryDescription => Category.ToString();
    }
}
