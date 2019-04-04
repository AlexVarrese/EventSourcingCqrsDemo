using AccountingApi.Domain;
using AccountingApi.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.EventsProcessingFunction
{
    public class AccountingEventHandlers
        : IRequestHandler<AccountEventRequest<AccountCreated>, Account>,
          IRequestHandler<AccountEventRequest<AccountClosed>, Account>,
          IRequestHandler<AccountEventRequest<BalanceIncreased>, Account>,
          IRequestHandler<AccountEventRequest<BalanceDecreased>, Account>
    {
        public IAccountQuerys AccountQuerys { get; }

        public AccountingEventHandlers(IAccountQuerys accountQuerys)
        {
            AccountQuerys = accountQuerys ?? throw new ArgumentNullException(nameof(accountQuerys));
        }

        public Task<Account> Handle(AccountEventRequest<AccountCreated> request, CancellationToken cancellationToken)
        {
            var account = new Account(request.Payload.AccountNumber, request.Payload.Owner);
            return Task.FromResult(account);
        }

        public async Task<Account> Handle(AccountEventRequest<AccountClosed> request, CancellationToken cancellationToken)
        {
            var account = await this.AccountQuerys.GetAccountByNumberAsync(request.Payload.AccountNumber);
            account.AccountState = AccountState.Closed;
            account.SequenceNumber = request.Payload.SequenceNumber;
            return account;
        }

        public async Task<Account> Handle(AccountEventRequest<BalanceIncreased> request, CancellationToken cancellationToken)
        {
            var account = await this.AccountQuerys.GetAccountByNumberAsync(request.Payload.AccountNumber);
            account.CurrentBalance += request.Payload.Amount;
            account.SequenceNumber = request.Payload.SequenceNumber;
            return account;
        }

        public async Task<Account> Handle(AccountEventRequest<BalanceDecreased> request, CancellationToken cancellationToken)
        {
            var account = await this.AccountQuerys.GetAccountByNumberAsync(request.Payload.AccountNumber);
            account.CurrentBalance -= request.Payload.Amount;
            account.SequenceNumber = request.Payload.SequenceNumber;
            return account;
        }
    }
}
