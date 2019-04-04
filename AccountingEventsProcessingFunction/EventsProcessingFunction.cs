using AccountingApi;
using AccountingApi.Domain;
using AccountingApi.Services;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountingEventsProcessingFunction
{
    public class EventsProcessingFunction
    {
        public IAccountQuerys AccountQuerys { get; }

        public EventsProcessingFunction(IAccountQuerys accountQuerys)
        {
            this.AccountQuerys = accountQuerys ?? throw new ArgumentNullException(nameof(accountQuerys));
        }
               

        [FunctionName("EventsProcessingFunction")]
        public async Task  Run([CosmosDBTrigger(
            databaseName: Constants.DatabaseName,
            collectionName: Constants.EventStoreCollectionName,
            ConnectionStringSetting = "CosmosDbConnection",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, 
            [CosmosDB(Constants.DatabaseName, Constants.ViewsCollectionName, ConnectionStringSetting = "CosmosDbConnection", Id = "Id")]IAsyncCollector<Aggregate> views,
            ILogger log)
        {
            foreach(var e in input)
            {
                var eventType = e.GetPropertyValue<string>(nameof(DomainEvent.Type));
                Account account = null;

                switch(eventType)
                {
                    case nameof(AccountCreated):
                        {
                            var accountCreatedEvent = JsonConvert.DeserializeObject<AccountCreated>(e.ToString());
                            account = new Account(accountCreatedEvent.AccountNumber, accountCreatedEvent.Owner);
                            break;
                        }
                    case nameof(AccountClosed):
                        {
                            var accountClosedEvent = JsonConvert.DeserializeObject<AccountClosed>(e.ToString());
                            account = await this.AccountQuerys.GetAccountByNumberAsync(accountClosedEvent.AccountNumber);
                            account.AccountState = AccountState.Closed;
                            account.SequenceNumber = accountClosedEvent.SequenceNumber;
                            break;
                        }
                    case nameof(BalanceIncreased):
                        {
                            var balanceIncreasedEvent = JsonConvert.DeserializeObject<BalanceIncreased>(e.ToString());
                            account = await this.AccountQuerys.GetAccountByNumberAsync(balanceIncreasedEvent.AccountNumber);
                            account.CurrentBalance += balanceIncreasedEvent.Amount;
                            account.SequenceNumber = balanceIncreasedEvent.SequenceNumber;
                            break;
                        }
                    case nameof(BalanceDecreased):
                        {
                            var balanceDecreasedEvent = JsonConvert.DeserializeObject<BalanceDecreased>(e.ToString());
                            account = await this.AccountQuerys.GetAccountByNumberAsync(balanceDecreasedEvent.AccountNumber);
                            account.CurrentBalance -= balanceDecreasedEvent.Amount;
                            account.SequenceNumber = balanceDecreasedEvent.SequenceNumber;
                            break;
                        }
                }

                await views.AddAsync(account);
            }
        }
    }
}
