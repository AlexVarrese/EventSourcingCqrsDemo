using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingApi.Infrastructure
{
    public class AggregateEvent
    {
        /// <summary>
        /// Unique Id of the event build from AggregateId and sequence number.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; }
        public string AggregateId { get; }
        public long SequenceNumber { get; }
        public string Type { get; }
        public DateTime CreatedAtUtc { get; }

        public AggregateEvent(string aggregateId, long sequenceNumber)
        {
            this.Id = $"{aggregateId}|{sequenceNumber}";
            this.Type = this.GetType().FullName;
            this.AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
            this.SequenceNumber = sequenceNumber;
            this.CreatedAtUtc = DateTime.UtcNow;
        }
    }
}
