using System.Threading.Tasks;
using AccountingApi.Domain;

namespace Accounting.Services
{
    public interface IAccountingEventHandlers
    {
        Task<Account> Handle(string eventType, string domainEvent);
    }
}