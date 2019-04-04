using System;

namespace AccountingApi.Domain
{
    public class AccountClosed : DomainEvent
    {
        public string AccountNumber { get; }

        public AccountClosed(string accountNumber, long sequenceNumber)
            : base($"{nameof(Account)}|{accountNumber}", sequenceNumber, nameof(AccountClosed))
        {
            this.AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
        }
    }
}
