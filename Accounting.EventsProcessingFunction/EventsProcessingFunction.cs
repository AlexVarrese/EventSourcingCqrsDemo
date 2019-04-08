using Accounting.EventsProcessingFunction;
using Accounting.Services;
using AccountingApi;
using AccountingApi.Domain;
using AccountingApi.Infrastructure;
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
        public IEventStore EventStore { get; }

        public EventsProcessingFunction(IEventStore eventStore)
        {
            this.EventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
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
                var aggregateId = e.GetPropertyValue<string>(nameof(AggregateEvent.AggregateId));

                var domainEvents = this.EventStore.GetDomainEvents(aggregateId);
                var account = new Account(domainEvents);

                await views.AddAsync(account);
            }
        }
    }
}
