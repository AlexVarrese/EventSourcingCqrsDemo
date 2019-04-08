using AccountingApi.Infrastructure;
using System;

namespace AccountingApi.Domain
{
    public class BalanceDecreased : DomainEvent
    {
        public string AccountNumber { get; }
        public double Amount { get; }

        public BalanceDecreased(string accountNumber, long sequenceNumber, double amount)
            : base($"{nameof(Account)}|{accountNumber}", sequenceNumber)
        {
            this.AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            this.Amount = amount;
        }
    }
}
