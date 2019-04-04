using System;

namespace AccountingApi.Domain
{
    public class BalanceIncreased : DomainEvent
    {
        public string AccountNumber { get; }
        public double Amount { get; }

        public BalanceIncreased(string accountNumber, long sequenceNumber, double amount) 
            : base($"{nameof(Account)}|{accountNumber}", sequenceNumber, nameof(BalanceIncreased))
        {
            this.AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            this.Amount = amount;
        }
    }
}
