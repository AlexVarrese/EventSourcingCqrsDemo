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
            if (account.AccountState == AccountState.Created)
            {
                throw new InvalidOperationException($"Account {command.AccountNumber} already exists.");
            }

            await this.EventStore.AddEventsAsync(new AccountCreated(command.AccountNumber, command.Owner));
        }

        public async Task MakeDepositAsync(MakeDepositCommand command)
        {
            var account =  BuildAccountFromDomainEvents(command.AccountNumber);

            await this.EventStore.AddEventsAsync(new BalanceIncreased(account.AccountNumber, ++account.SequenceNumber, command.Amount));
        }

        public async Task TransferMoneyAsync(TransferMoneyCommand command)
        {
            var sourceAccount = BuildAccountFromDomainEvents(command.SourceAccountNumber);
            var destinationAccount = BuildAccountFromDomainEvents(command.DestinationAccountNumber);

            await this.EventStore.AddEventsAsync(
                new BalanceDecreased(sourceAccount.AccountNumber, ++sourceAccount.SequenceNumber, command.Amount),
                new BalanceIncreased(destinationAccount.AccountNumber, ++destinationAccount.SequenceNumber, command.Amount));
        }

        private Account BuildAccountFromDomainEvents(string accountNumber)
        {
            var domainEvents = this.EventStore.GetDomainEvents(Account.CreateAggregateId(accountNumber));
            var account = new Account(domainEvents);
            return account;
        }
    }
}
