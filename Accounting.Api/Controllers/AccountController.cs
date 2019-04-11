using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounting.Services.Commands;
using AccountingApi.Domain;
using AccountingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountingApi.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IAccountCommands AccountCommands { get; }

        public IAccountQuerys AccountQuerys { get; }


        public AccountController(IAccountCommands commands, IAccountQuerys querys)
        {
            AccountCommands = commands ?? throw new ArgumentNullException(nameof(commands));
            AccountQuerys = querys ?? throw new ArgumentNullException(nameof(querys));
        }

        [HttpGet("{accountNumber}")]
        public async Task<Account> GetAccount(string accountNumber)
        {
            var result = await this.AccountQuerys.GetAccountByNumberAsync(accountNumber);
            return result;
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
            await this.AccountCommands.CloseAccountAsync(new CloseAccountCommand(accountNumber));
        }
    }
}
