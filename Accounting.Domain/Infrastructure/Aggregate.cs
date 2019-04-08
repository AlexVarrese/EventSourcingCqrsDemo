﻿using System;

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
        public string AggregateId { get; }

        public long SequenceNumber { get; set; }

        public Aggregate(string id, long sequenceNumber)
        {
            this.AggregateId = id ?? throw new ArgumentNullException(nameof(id));
            this.SequenceNumber = sequenceNumber;
        }
    }
}