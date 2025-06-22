using BankingSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingSystem.Domain.Entities
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountHolderName { get; set; } = string.Empty;
        public List<Transaction> Transactions { get; set; } = new();

        [NotMapped]
        public long CurrentBalance
        {
            get
            {
                return Transactions.Sum(t =>
                    t.Type == TransactionType.Credit ? t.Amount : -t.Amount
                );
            }
        }
    }
}
