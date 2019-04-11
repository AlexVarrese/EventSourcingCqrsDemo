using System.Collections.Generic;
using System.Threading.Tasks;
using AccountingApi.Domain;

namespace AccountingApi.Infrastructure
{
    public interface IEventStore
    {
        Task AddEventsAsync(IEnumerable<AggregateEvent> aggregateEvents);
        IEnumerable<AggregateEvent> GetAggregateEvents(string aggregateId);
    }
}