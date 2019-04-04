using System.Threading.Tasks;
using AccountingApi.Domain;

namespace Accounting.EventsProcessingFunction
{
    public interface IAccountingEventHandlers
    {
        Task<Account> Handle(string type, string domainEvent);
        Task<Account> Handle(AccountClosed request);
        Task<Account> Handle(AccountCreated request);
        Task<Account> Handle(BalanceDecreased request);
        Task<Account> Handle(BalanceIncreased request);
    }
}