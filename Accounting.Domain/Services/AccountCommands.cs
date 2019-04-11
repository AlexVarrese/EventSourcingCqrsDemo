using Accounting.Services.Commands;
using AccountingApi.Domain;
using AccountingApi.Infrastructure;
using System;
using System.Linq;
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

        public async Task CloseAccountAsync(CloseAccountCommand command)
        {
            var account = BuildAccountFromDomainEvents(command.AccountNumber);
            var events = account.CloseAccount(command);
            await this.EventStore.AddEventsAsync(events);
        }

        public async Task CreateAccountAsync(CreateAccountCommand command)
        {
            var account = BuildAccountFromDomainEvents(command.AccountNumber);
            var events = account.CreateAccount(command);
            await this.EventStore.AddEventsAsync(events);
        }

        public async Task MakeDepositAsync(MakeDepositCommand command)
        {
            var account =  BuildAccountFromDomainEvents(command.AccountNumber);
            var events = account.MakeDeposit(command);
            await this.EventStore.AddEventsAsync(events);
        }

        public async Task TransferMoneyAsync(TransferMoneyCommand command)
        {
            var sourceAccount = BuildAccountFromDomainEvents(command.SourceAccountNumber);
            var events = sourceAccount.TransferMoney(command);
            await this.EventStore.AddEventsAsync(events);
        }

        private Account BuildAccountFromDomainEvents(string accountNumber)
        {
            var domainEvents = this.EventStore.GetAggregateEvents(Account.CreateAggregateId(accountNumber));
            var account = new Account(this.EventStore, domainEvents);
            return account;
        }
    }
}
