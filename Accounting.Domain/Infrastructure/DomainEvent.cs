using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingApi.Infrastructure
{
    public class DomainEvent
    {
        /// <summary>
        /// Unique Id of the event build from AggregateId and sequence number.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        public string AggregateId { get; set; }
        public long SequenceNumber { get; set; }
        public string Type { get; set; }

        public DomainEvent(string aggregateId, long sequenceNumber)
        {
            this.Id = $"{aggregateId}|{sequenceNumber}";
            this.Type = this.GetType().FullName;
            this.AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
            this.SequenceNumber = sequenceNumber;
        }
    }
}
