using Accounting.Domain;
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

        public async Task<Account> Handle(string type, string eventJson)
        {
            Account account = null;

            var eventObject = JsonConvert.DeserializeObject(eventJson.ToString(), Type.GetType(type));

            switch (eventObject)
            {
                case AccountCreated accountCreatedEvent:
                    {
                        account = new AccountAggregateRoot().Handle(accountCreatedEvent);
                        break;
                    }
                case AccountClosed accountClosedEvent:
                    {
                        account = await this.AccountQuerys.GetAccountByNumberAsync(accountClosedEvent.AccountNumber);
                        account = new AccountAggregateRoot().Handle(account, accountClosedEvent);
                        break;
                    }
                case BalanceIncreased balanceIncreasedEvent:
                    {
                        account = await this.AccountQuerys.GetAccountByNumberAsync(balanceIncreasedEvent.AccountNumber);
                        account = new AccountAggregateRoot().Handle(account, balanceIncreasedEvent);

                        break;
                    }
                case BalanceDecreased balanceDecreasedEvent:
                    {
                        account = await this.AccountQuerys.GetAccountByNumberAsync(balanceDecreasedEvent.AccountNumber);
                        account = new AccountAggregateRoot().Handle(account, balanceDecreasedEvent);

                        break;
                    }
            }

            return account;
        }
    }
}
