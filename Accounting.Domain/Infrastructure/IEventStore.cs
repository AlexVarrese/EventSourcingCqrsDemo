using System.Collections.Generic;
using System.Threading.Tasks;
using AccountingApi.Domain;

namespace AccountingApi.Infrastructure
{
    public interface IEventStore
    {
        Task AddEventsAsync(params AggregateEvent[] domainEvents);
        IEnumerable<AggregateEvent> GetDomainEvents(string aggregateId);
    }
}