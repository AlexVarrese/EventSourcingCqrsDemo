using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounting.Services.Commands;
using AccountingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountingApi.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountCommandController : ControllerBase
    {
        public IAccountCommands AccountCommands { get; }

        public AccountCommandController(IAccountCommands commands)
        {
            AccountCommands = commands ?? throw new ArgumentNullException(nameof(commands));
        }
        
        [HttpPost("create")]
        public async Task Create([FromBody] CreateAccountCommand command)
        {
            await this.AccountCommands.CreateAccountAsync(command);
        }

        [HttpPost("deposit")]
        public async Task Deposit([FromBody]MakeDepositCommand command)
        {
            await this.AccountCommands.MakeDepositAsync(command);
        }

        [HttpPost("transfer")]
        public async Task TransferMoney([FromBody]TransferMoneyCommand command)
        {
            await this.AccountCommands.TransferMoneyAsync(command);
        }

        // DELETE api/values/5
        [HttpPost("{accountNumber}/close")]
        public async Task Close(string accountNumber)
        {
            await this.AccountCommands.CloseAccountAsync(new CloseAccoundCommand(accountNumber));
        }
    }
}
