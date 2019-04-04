using System;

namespace AccountingApi.Domain
{
    public class AccountCreated : DomainEvent
    {
        public string AccountNumber { get; }
        public string Owner { get; set; }

        public AccountCreated(string accountNumber, string owner, long sequenceNumber = 0)
            : base($"{nameof(Account)}|{accountNumber}", sequenceNumber, nameof(AccountCreated))
        {
            this.AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
            this.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
    }
}
