using AccountingApi.Domain;
using AccountingApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Domain
{
    public class AccountAggregateRoot
    {
        public Account Aggregate { get;  }

        public AccountAggregateRoot()
        {

        }

        public AccountAggregateRoot(IEnumerable<DomainEvent> domainEvents)
        {
            this.Aggregate = BuildFromDomainEvents(domainEvents);
        }

        public Account BuildFromDomainEvents(IEnumerable<DomainEvent> domainEvents)
        {
            Account account = null;
            foreach (var eventObject in domainEvents)
            {
                switch (eventObject)
                {
                    case AccountCreated accountCreatedEvent:
                        {
                            account = Handle(accountCreatedEvent);
                            break;
                        }
                    case AccountClosed accountClosedEvent:
                        {
                            account = Handle(account, accountClosedEvent);
                            break;
                        }
                    case BalanceIncreased balanceIncreasedEvent:
                        {
                            account = Handle(account, balanceIncreasedEvent);

                            break;
                        }
                    case BalanceDecreased balanceDecreasedEvent:
                        {
                            account = Handle(account, balanceDecreasedEvent);

                            break;
                        }
                }
            }

            return account;
        }

        public Account Handle(AccountCreated request)
        {
            var account = new Account(request.AccountNumber, request.Owner);
            return account;
        }

        public Account Handle(Account account, AccountClosed request)
        {
            account.AccountState = AccountState.Closed;
            account.SequenceNumber = request.SequenceNumber;
            return account;
        }

        public Account Handle(Account account, BalanceIncreased request)
        {
            account.CurrentBalance += request.Amount;
            account.SequenceNumber = request.SequenceNumber;
            return account;
        }

        public Account Handle(Account account, BalanceDecreased request)
        {
            account.CurrentBalance -= request.Amount;
            account.SequenceNumber = request.SequenceNumber;
            return account;
        }
    }
}
