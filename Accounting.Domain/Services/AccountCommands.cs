using Accounting.Services.Commands;
using AccountingApi.Domain;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingApi.Services
{
    public class AccountCommands : IAccountCommands
    {
        private DocumentClient DocumentClient { get; }
        private IAccountQuerys AccountQuerys { get; }

        public AccountCommands(DocumentClient documentClient, IAccountQuerys accountQuerys)
        {
            this.DocumentClient = documentClient ?? throw new ArgumentNullException(nameof(documentClient));
            this.AccountQuerys = accountQuerys ?? throw new ArgumentNullException(nameof(accountQuerys));
        }

        public async Task CloseAccountAsync(CloseAccoundCommand command)
        {
            var account = await AccountQuerys.GetAccountByNumberAsync(command.AccountNumber);
            if (account.AccountState == AccountState.Closed)
            {
                throw new InvalidOperationException($"Account {command.AccountNumber} is already closed.");
            }

            await DispatchEvents(new AccountClosed(command.AccountNumber, ++account.SequenceNumber));
        }

        public async Task CreateAccountAsync(CreateAccountCommand command)
        {
            if(AccountQuerys.IsAccountExisting(command.AccountNumber))
            {
                throw new InvalidOperationException($"Account {command.AccountNumber} already exists.");
            }

            await DispatchEvents(new AccountCreated(command.AccountNumber, command.Owner));
        }

        public async Task MakeDepositAsync(MakeDepositCommand command)
        {
            var account = await AccountQuerys.GetAccountByNumberAsync(command.AccountNumber);
            if(account.AccountState == AccountState.Closed)
            {
                throw new InvalidOperationException($"Account {command.AccountNumber} is closed.");
            }

            await DispatchEvents(new BalanceIncreased(account.AccountNumber, ++account.SequenceNumber, command.Amount));
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

            await DispatchEvents(
                new BalanceDecreased(sourceAccount.AccountNumber, ++sourceAccount.SequenceNumber, command.Amount),
                new BalanceIncreased(destinationAccount.AccountNumber, ++destinationAccount.SequenceNumber, command.Amount));
        }

        private async Task DispatchEvents(params DomainEvent[] domainEvents)
        {
            foreach (var e in domainEvents)
            {
                await DocumentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.DatabaseName, Constants.EventStoreCollectionName), e);
            }
        }

    }
}
