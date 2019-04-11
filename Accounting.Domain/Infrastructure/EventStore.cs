using AccountingApi;
using AccountingApi.Domain;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingApi.Infrastructure
{
    public class EventStore : IEventStore
    {
        private DocumentClient DocumentClient { get; }

        public EventStore(DocumentClient documentClient)
        {
            this.DocumentClient = documentClient ?? throw new ArgumentNullException(nameof(documentClient));
        }

        public async Task AddEventsAsync(IEnumerable<AggregateEvent> aggregateEvents)
        {
            foreach (var e in aggregateEvents)
            {
                await DocumentClient.CreateDocumentAsync(GetEventStoreUri(), e);
            }
        }

        public IEnumerable<AggregateEvent> GetAggregateEvents(string aggregateId)
        {
            var query = DocumentClient.CreateDocumentQuery(GetEventStoreUri(), new FeedOptions() { PartitionKey = new Microsoft.Azure.Documents.PartitionKey(aggregateId) });
            
            return query.ToList().Select(d =>
                (AggregateEvent)JsonConvert.DeserializeObject(d.ToString(), Type.GetType(d.GetPropertyValue<string>(nameof(AggregateEvent.Type))))
            ).OrderBy(d=>d.SequenceNumber).ToList();
        }

        private Uri GetEventStoreUri()
        {
            return UriFactory.CreateDocumentCollectionUri(Constants.DatabaseName, Constants.EventStoreCollectionName);
        }
    }
}
