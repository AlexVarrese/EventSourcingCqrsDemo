﻿using AccountingApi.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AccountingApi.Domain
{
    public class Account : Aggregate
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public double CurrentBalance { get; set; }
        public AccountState AccountState { get; set; }
        public string Owner { get; set; }

        public static string CreateAggregateId(string accountNumber)
        {
            return $"{nameof(Account)}|{accountNumber}";
        }

        public Account()
        {
        }

        public Account(IEnumerable<AggregateEvent> domainEvents)
        {
            this.ApplyEvents(domainEvents);
        }

        public Account ApplyEvents(IEnumerable<AggregateEvent> domainEvents)
        {
            Account account = null;
            foreach (var eventObject in domainEvents)
            {
                switch (eventObject)
                {
                    case AccountCreated accountCreatedEvent:
                        {
                            CreateAccount(accountCreatedEvent);
                            break;
                        }
                    case AccountClosed accountClosedEvent:
                        {
                            CloseAccount(accountClosedEvent);
                            break;
                        }
                    case BalanceIncreased balanceIncreasedEvent:
                        {
                            IncreaseBalance(balanceIncreasedEvent);

                            break;
                        }
                    case BalanceDecreased balanceDecreasedEvent:
                        {
                            DecreaseBalance(balanceDecreasedEvent);

                            break;
                        }
                }
            }

            return account;
        }

        public void CreateAccount(AccountCreated request)
        {
            this.Id = CreateAggregateId(request.AccountNumber);
            this.AggregateId = CreateAggregateId(request.AccountNumber);
            this.AccountNumber = request.AccountNumber;
            this.Owner = request.Owner;
            this.AccountState = AccountState.Created;            
        }

        public void CloseAccount(AccountClosed request)
        {
            if (this.AccountState == AccountState.Closed)
            {
                throw new InvalidOperationException($"Account {AccountNumber} is already closed.");
            }

            this.AccountState = AccountState.Closed;
            this.SequenceNumber = request.SequenceNumber;
        }

        public void IncreaseBalance(BalanceIncreased request)
        {
            if (this.AccountState == AccountState.Closed)
            {
                throw new InvalidOperationException($"Account {AccountNumber} is closed.");
            }

            this.CurrentBalance += request.Amount;
            this.SequenceNumber = request.SequenceNumber;
        }

        public void DecreaseBalance(BalanceDecreased request)
        {
            if (this.AccountState == AccountState.Closed)
            {
                throw new InvalidOperationException($"Account {AccountNumber} is closed.");
            }
            if (this.CurrentBalance < request.Amount)
            {
                throw new InvalidOperationException($"Account {AccountNumber} does not have enough balance to execute the transaction.");
            }
            this.CurrentBalance -= request.Amount;
            this.SequenceNumber = request.SequenceNumber;
        }
    }
}
