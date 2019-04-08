using Accounting.Services.Commands;
using AccountingApi.Domain;
using AccountingApi.Infrastructure;
using System;
using System.Threading.Tasks;

namespace AccountingApi.Services
{
    public class AccountCommands : IAccountCommands
    {
        public IAccountQuerys AccountQuerys { get; }
        public IEventStore EventStore { get; }

        public AccountCommands(IAccountQuerys accountQuerys, IEventStore eventStore)

        {
            this.AccountQuerys = accountQuerys ?? throw new ArgumentNullException(nameof(accountQuerys));
            this.EventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }

        public async Task CloseAccountAsync(CloseAccoundCommand command)
        {
            var account = await AccountQuerys.GetAccountByNumberAsync(command.AccountNumber);
            if (account.AccountState == AccountState.Closed)
            {
                throw new InvalidOperationException($"Account {command.AccountNumber} is already closed.");
            }

            await this.EventStore.AddEventsAsync(new AccountClosed(command.AccountNumber, ++account.SequenceNumber));
        }

        public async Task CreateAccountAsync(CreateAccountCommand command)
        {
            if(AccountQuerys.IsAccountExisting(command.AccountNumber))
            {
                throw new InvalidOperationException($"Account {command.AccountNumber} already exists.");
            }

            await this.EventStore.AddEventsAsync(new AccountCreated(command.AccountNumber, command.Owner));
        }

        public async Task MakeDepositAsync(MakeDepositCommand command)
        {
            var account = await AccountQuerys.GetAccountByNumberAsync(command.AccountNumber);
            if(account.AccountState == AccountState.Closed)
            {
                throw new InvalidOperationException($"Account {command.AccountNumber} is closed.");
            }

            await this.EventStore.AddEventsAsync(new BalanceIncreased(account.AccountNumber, ++account.SequenceNumber, command.Amount));
        }

        public async Task TransferMoneyAsync(TransferMoneyCommand command)
        {
            var sourceAccount = await AccountQuerys.GetAccountByNumberAsync(command.SourceAccountNumber);
            if (sourceAccount.AccountState == AccountState.Closed)
            {
                throw new InvalidOperationException($"Account {command.SourceAccountNumber} is closed.");
            }
            if (sourceAccount.CurrentBalance < command.Amount)
            {
                throw new InvalidOperationException($"Account {command.SourceAccountNumber} does not have enough balance to execute the transaction.");
            }
            var destinationAccount = await AccountQuerys.GetAccountByNumberAsync(command.DestinationAccountNumber);
            if (destinationAccount.AccountState == AccountState.Closed)
            {
                throw new InvalidOperationException($"Account {command.DestinationAccountNumber} is closed.");
            }

            await this.EventStore.AddEventsAsync(
                new BalanceDecreased(sourceAccount.AccountNumber, ++sourceAccount.SequenceNumber, command.Amount),
                new BalanceIncreased(destinationAccount.AccountNumber, ++destinationAccount.SequenceNumber, command.Amount));
        }

        

    }
}
