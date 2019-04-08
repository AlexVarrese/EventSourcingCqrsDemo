using System.Collections.Generic;
using System.Threading.Tasks;
using AccountingApi.Domain;

namespace AccountingApi.Infrastructure
{
    public interface IEventStore
    {
        Task AddEventsAsync(params DomainEvent[] domainEvents);
        IEnumerable<DomainEvent> GetDomainEvents(string aggregateId);
    }
}