using AccountingApi;
using AccountingApi.Domain;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Domain.Services
{
    public class EventStore : IEventStore
    {
        private DocumentClient DocumentClient { get; }

        public EventStore(DocumentClient documentClient)
        {
            this.DocumentClient = documentClient ?? throw new ArgumentNullException(nameof(documentClient));
        }

        public async Task AddEventsAsync(params DomainEvent[] domainEvents)
        {
            foreach (var e in domainEvents)
            {
                await DocumentClient.CreateDocumentAsync(GetEventStoreUri(), e);
            }
        }

        public IEnumerable<DomainEvent> GetDomainEvents(string aggregateId)
        {
            var query = DocumentClient.CreateDocumentQuery(GetEventStoreUri(), new FeedOptions() { PartitionKey = new Microsoft.Azure.Documents.PartitionKey(aggregateId) });
            
            return query.ToList().Select(d =>
                (DomainEvent)JsonConvert.DeserializeObject(d.ToString(), Type.GetType(d.GetPropertyValue<string>(nameof(DomainEvent.Type))))
            ).ToList();
        }

        private Uri GetEventStoreUri()
        {
            return UriFactory.CreateDocumentCollectionUri(Constants.DatabaseName, Constants.EventStoreCollectionName);
        }
    }
}
