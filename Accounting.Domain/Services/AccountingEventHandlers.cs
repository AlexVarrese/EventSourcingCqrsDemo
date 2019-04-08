using AccountingApi.Domain;
using AccountingApi.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Accounting.Services
{
    public class AccountingEventHandlers : IAccountingEventHandlers
    {
        public IAccountQuerys AccountQuerys { get; }

        public AccountingEventHandlers(IAccountQuerys accountQuerys)
        {
            AccountQuerys = accountQuerys ?? throw new ArgumentNullException(nameof(accountQuerys));
        }

        public Task<Account> Handle(AccountCreated request)
        {
            var account = new Account(request.AccountNumber, request.Owner);
            return Task.FromResult(account);
        }

        public async Task<Account> Handle(AccountClosed request)
        {
            var account = await this.AccountQuerys.GetAccountByNumberAsync(request.AccountNumber);
            account.AccountState = AccountState.Closed;
            account.SequenceNumber = request.SequenceNumber;
            return account;
        }

        public async Task<Account> Handle(BalanceIncreased request)
        {
            var account = await this.AccountQuerys.GetAccountByNumberAsync(request.AccountNumber);
            account.CurrentBalance += request.Amount;
            account.SequenceNumber = request.SequenceNumber;
            return account;
        }

        public async Task<Account> Handle(BalanceDecreased request)
        {
            var account = await this.AccountQuerys.GetAccountByNumberAsync(request.AccountNumber);
            account.CurrentBalance -= request.Amount;
            account.SequenceNumber = request.SequenceNumber;
            return account;
        }

        public async Task<Account> Handle(string type, string eventJson)
        {
            Account account = null;

            var eventObject = JsonConvert.DeserializeObject(eventJson.ToString(), Type.GetType(type));

            switch (eventObject)
            {
                case AccountCreated accountCreatedEvent:
                    {
                        account = await Handle(accountCreatedEvent);
                        break;
                    }
                case AccountClosed accountClosedEvent:
                    {
                        account = await Handle(accountClosedEvent);
                        break;
                    }
                case BalanceIncreased balanceIncreasedEvent:
                    {
                        account = await Handle(balanceIncreasedEvent);

                        break;
                    }
                case BalanceDecreased balanceDecreasedEvent:
                    {
                        account = await Handle(balanceDecreasedEvent);

                        break;
                    }
            }

            return account;
        }
    }
}
