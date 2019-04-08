using AccountingApi.Infrastructure;
using System;

namespace AccountingApi.Domain
{
    public class AccountClosed : AggregateEvent
    {
        public string AccountNumber { get; }

        public AccountClosed(string accountNumber, long sequenceNumber)
            : base($"{nameof(Account)}|{accountNumber}", sequenceNumber)
        {
            this.AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
        }
    }
}
