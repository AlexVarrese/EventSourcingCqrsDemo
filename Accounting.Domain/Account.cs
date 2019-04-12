using Accounting.Services.Commands;
using AccountingApi.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AccountingApi.Domain
{
    public class Account : Aggregate
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public double CurrentBalance { get; set; }
        public AccountState AccountState { get; set; }
        public string Owner { get; set; }


        public static string CreateAggregateId(string accountNumber)
        {
            return $"{nameof(Account)}|{accountNumber}";
        }

        public Account()
        {

        }

        public Account(IEventStore eventStore) : base(eventStore)
        {
        }

        public Account(IEventStore eventStore, IEnumerable<AggregateEvent> domainEvents) 
            : base(eventStore)
        {
            this.ApplyEvents(domainEvents);
        }

        public Account ApplyEvents(IEnumerable<AggregateEvent> domainEvents)
        {
            Account account = null;
            foreach (var eventObject in domainEvents)
            {
                switch (eventObject)
                {
                    case AccountCreated accountCreatedEvent:
                        {
                            Handle(accountCreatedEvent);
                            break;
                        }
                    case AccountClosed accountClosedEvent:
                        {
                            Handle(accountClosedEvent);
                            break;
                        }
                    case BalanceIncreased balanceIncreasedEvent:
                        {
                            Handle(balanceIncreasedEvent);

                            break;
                        }
                    case BalanceDecreased balanceDecreasedEvent:
                        {
                            Handle(balanceDecreasedEvent);

                            break;
                        }
                }
            }

            return account;
        }

        #region Event Handlers


        public void Handle(AccountCreated request)
        {
            this.Id = CreateAggregateId(request.AccountNumber);
            this.AggregateId = CreateAggregateId(request.AccountNumber);
            this.AccountNumber = request.AccountNumber;
            this.Owner = request.Owner;
            this.AccountState = AccountState.Created;            
        }

        public void Handle(AccountClosed request)
        {
            this.AccountState = AccountState.Closed;
            this.SequenceNumber = request.SequenceNumber;
        }

        public void Handle(BalanceIncreased request)
        {
            this.CurrentBalance += request.Amount;
            this.SequenceNumber = request.SequenceNumber;
        }

        public void Handle(BalanceDecreased request)
        {
            this.CurrentBalance -= request.Amount;
            this.SequenceNumber = request.SequenceNumber;
        }

        #endregion

        #region Command Handlers

        public IEnumerable<AggregateEvent> CreateAccount(CreateAccountCommand command)
        {
            var accountEvents = this.EventStore.GetAggregateEvents(CreateAggregateId(command.AccountNumber));
            var account = new Account(this.EventStore, accountEvents);
            if (account.AccountState == AccountState.Created)
            {
                throw new InvalidOperationException($"Account {AccountNumber} already exists.");
            }

            var aggregateEvent = new AccountCreated(command.AccountNumber, command.Owner);
            Handle(aggregateEvent);


            return new AggregateEvent[]
            {
                aggregateEvent
            };
        }

        public IEnumerable<AggregateEvent> CloseAccount(CloseAccountCommand command)
        {
            if (this.AccountState == AccountState.Closed)
            {
                throw new InvalidOperationException($"Account {AccountNumber} is already closed.");
            }

            if(this.CurrentBalance > 0)
            {
                throw new InvalidOperationException($"Account {AccountNumber} still has money on it. Please withdraw before closing.");
            }

            var aggregateEvent = new AccountClosed(command.AccountNumber, ++SequenceNumber);
            Handle(aggregateEvent);

            return new AggregateEvent[]
            {
                aggregateEvent
            };
        }

        public IEnumerable<AggregateEvent> MakeDeposit(MakeDepositCommand command)
        {
            if(this.AccountState != AccountState.Created)
            {
                throw new InvalidOperationException($"Account {AccountNumber} is not available.");
            }

            var aggregateEvent = new BalanceIncreased(command.AccountNumber, ++SequenceNumber, command.Amount);
            Handle(aggregateEvent);

            return new AggregateEvent[]
            {
                aggregateEvent
            };
        }

        public IEnumerable<AggregateEvent> TransferMoney(TransferMoneyCommand command)
        {
            if (this.CurrentBalance < command.Amount)
            {
                throw new InvalidOperationException($"Account {AccountNumber} does not have enough balance to execute the transaction.");
            }
            if(this.AccountState != AccountState.Created)
            {
                throw new InvalidOperationException($"Account {AccountNumber} is not available.");
            }

            var destinationAccountEvents = this.EventStore.GetAggregateEvents(CreateAggregateId(command.DestinationAccountNumber));
            var destinationAccount = new Account(this.EventStore, destinationAccountEvents);
            if(destinationAccount.AccountState != AccountState.Created)
            {
                throw new InvalidOperationException($"Account {destinationAccount.AccountNumber} is not available.");
            }

            var aggregateEvent = new BalanceIncreased(command.DestinationAccountNumber, ++destinationAccount.SequenceNumber, command.Amount);
            Handle(aggregateEvent);

            return new AggregateEvent[]
            {
                aggregateEvent,
                new BalanceDecreased(command.SourceAccountNumber, ++this.SequenceNumber, command.Amount)
            };
        }

        #endregion
    }
}
