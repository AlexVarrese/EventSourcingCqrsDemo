using Accounting.Services.Commands;
using AccountingApi.Domain;
using AccountingApi.Infrastructure;
using System;
using System.Threading.Tasks;

namespace AccountingApi.Services
{
    public class AccountCommands : IAccountCommands
    {
        public IEventStore EventStore { get; }

        public AccountCommands(IEventStore eventStore)
        {
            this.EventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }

        public async Task CloseAccountAsync(CloseAccoundCommand command)
        {
            var account = BuildAccountFromDomainEvents(command.AccountNumber);

            await this.EventStore.AddEventsAsync(new AccountClosed(command.AccountNumber, ++account.SequenceNumber));
        }

        public async Task CreateAccountAsync(CreateAccountCommand command)
        {
            var account = BuildAccountFromDomainEvents(command.AccountNumber);

            var aggregateEvent = new AccountCreated(command.AccountNumber, command.Owner);
            account.CreateAccount(aggregateEvent);
                        
            await this.EventStore.AddEventsAsync(aggregateEvent);
        }

        public async Task MakeDepositAsync(MakeDepositCommand command)
        {
            var account =  BuildAccountFromDomainEvents(command.AccountNumber);

            var aggregateEvent = new BalanceIncreased(account.AccountNumber, ++account.SequenceNumber, command.Amount);
            account.IncreaseBalance(aggregateEvent);

            await this.EventStore.AddEventsAsync(aggregateEvent);
        }

        public async Task TransferMoneyAsync(TransferMoneyCommand command)
        {
            var sourceAccount = BuildAccountFromDomainEvents(command.SourceAccountNumber);
            var decreasedEvent = new BalanceDecreased(sourceAccount.AccountNumber, ++sourceAccount.SequenceNumber, command.Amount);
            sourceAccount.DecreaseBalance(decreasedEvent);

            var destinationAccount = BuildAccountFromDomainEvents(command.DestinationAccountNumber);
            var increasedEvent = new BalanceIncreased(destinationAccount.AccountNumber, ++destinationAccount.SequenceNumber, command.Amount);
            destinationAccount.IncreaseBalance(increasedEvent);

            await this.EventStore.AddEventsAsync(decreasedEvent, increasedEvent);
        }

        private Account BuildAccountFromDomainEvents(string accountNumber)
        {
            var domainEvents = this.EventStore.GetDomainEvents(Account.CreateAggregateId(accountNumber));
            var account = new Account(domainEvents);
            return account;
        }
    }
}
