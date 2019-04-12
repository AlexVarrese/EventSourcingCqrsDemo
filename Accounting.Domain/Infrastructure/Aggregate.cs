using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AccountingApi.Infrastructure
{
    /// <summary>
    /// Reflects a projection of related domain events.
    /// </summary>
    public abstract class Aggregate
    {
        /// <summary>
        /// It is important to partition your event store by this key, as this will guarantee that events will get processed in sync.
        /// </summary>
        public string AggregateId { get; set; }

        public long SequenceNumber { get; set; }

        [JsonIgnore]
        public IEventStore EventStore { get; }

        public Aggregate()
        {

        }

        public Aggregate(IEventStore eventStore)
        {
            this.EventStore = eventStore;
        }
    }
}
