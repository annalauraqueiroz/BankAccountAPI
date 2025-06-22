using BankingSystem.Domain.Enums;

namespace BankingSystem.Domain.Entities
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public DateTime Date { get; private set; }
        public int AccountId { get; private set; }
        public long Amount { get; private set; }
        public string Description { get; private set; }
        public TransactionType Type { get; private set; }
        public TransactionCategory Category { get; private set; }
        public Transaction(int accountId, long amount, string description, TransactionType type, TransactionCategory category)
        {
            TransactionId = Guid.NewGuid();
            Date = DateTime.Now;
            AccountId = accountId;
            Amount = amount;
            Description = description;
            Type = type;
            Category = category;
        }
    }

}
