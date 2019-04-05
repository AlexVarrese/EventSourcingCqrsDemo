using AccountingApi.Domain;
using System.Threading.Tasks;

namespace AccountingApi.Services
{
    public interface IAccountQuerys
    {
        bool IsAccountExisting(string accountNumber);
        Task<Account> GetAccountByNumberAsync(string accountNumber);
    }
}
