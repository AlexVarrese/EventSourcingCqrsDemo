using Accounting.EventsProcessingFunction;
using AccountingApi;
using AccountingApi.Domain;
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
        public IAccountingEventHandlers EventHandler { get; }

        public EventsProcessingFunction(IAccountingEventHandlers eventHandler)
        {
            this.EventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
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
                Account account = await EventHandler.Handle(eventType, e.ToString());

                await views.AddAsync(account);
            }
        }
    }
}
