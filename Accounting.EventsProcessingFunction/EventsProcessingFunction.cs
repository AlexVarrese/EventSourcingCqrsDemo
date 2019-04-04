using Accounting.EventsProcessingFunction;
using AccountingApi;
using AccountingApi.Domain;
using AccountingApi.Services;
using MediatR;
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
        public IMediator Mediator { get; }

        public EventsProcessingFunction(IAccountQuerys accountQuerys, IMediator mediator)
        {
            this.Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
               

        [FunctionName("EventsProcessingFunction")]
        public async Task  Run([CosmosDBTrigger(
            databaseName: Constants.DatabaseName,
            collectionName: Constants.EventStoreCollectionName,
            ConnectionStringSetting = "CosmosDbConnection",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> events, 
            [CosmosDB(Constants.DatabaseName, Constants.ViewsCollectionName, ConnectionStringSetting = "CosmosDbConnection", Id = "Id")]IAsyncCollector<Aggregate> views,
            ILogger log)
        {
            foreach(var e in events)
            {
                var eventType = e.GetPropertyValue<string>(nameof(DomainEvent.Type));
                Account account = null;
                
                switch(eventType)
                {
                    case nameof(AccountCreated):
                        {
                            var accountCreatedEvent = JsonConvert.DeserializeObject<AccountCreated>(e.ToString());
                            account = await Mediator.Send(new AccountEventRequest<AccountCreated>(accountCreatedEvent));
                            break;
                        }
                    case nameof(AccountClosed):
                        {
                            var accountClosedEvent = JsonConvert.DeserializeObject<AccountClosed>(e.ToString());
                            account = await Mediator.Send(new AccountEventRequest<AccountClosed>(accountClosedEvent));
                            break;
                        }
                    case nameof(BalanceIncreased):
                        {
                            var balanceIncreasedEvent = JsonConvert.DeserializeObject<BalanceIncreased>(e.ToString());
                            account = await Mediator.Send(new AccountEventRequest<BalanceIncreased>(balanceIncreasedEvent));

                            break;
                        }
                    case nameof(BalanceDecreased):
                        {
                            var balanceDecreasedEvent = JsonConvert.DeserializeObject<BalanceDecreased>(e.ToString());
                            account = await Mediator.Send(new AccountEventRequest<BalanceDecreased>(balanceDecreasedEvent));

                            break;
                        }
                }

                await views.AddAsync(account);
            }
        }
    }
}
