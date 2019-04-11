using Accounting.Services.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingApi.Services
{
    public interface IAccountCommands
    {
        Task CreateAccountAsync(CreateAccountCommand command);
        Task MakeDepositAsync(MakeDepositCommand command);
        Task TransferMoneyAsync(TransferMoneyCommand command);
        Task CloseAccountAsync(CloseAccountCommand command);
    }
}
