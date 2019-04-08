using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingApi.Domain
{
    public class Account : Aggregate
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public double CurrentBalance { get; set; }
        public AccountState AccountState { get; set; }
        public string Owner { get; }

        public static string CreateAggregateId(string accountNumber)
        {
            return $"{nameof(Account)}|{accountNumber}";
        }

        public Account(string accountNumber, string owner, long sequenceNumber = 0) 
            : base(CreateAggregateId(accountNumber), sequenceNumber)
        {
            this.Id = CreateAggregateId(accountNumber);
            this.AccountNumber = accountNumber;
            this.Owner = owner;
            this.AccountState = AccountState.Created;
        }
    }
}
