using AccountingApi.Domain;
using AccountingApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Api.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountQueryController : ControllerBase
    {
        public IAccountQuerys AccountQuerys { get; }

        public AccountQueryController(IAccountQuerys querys)
        {
            AccountQuerys = querys ?? throw new ArgumentNullException(nameof(querys));
        }

        // POST api/values
        [HttpGet("{accountNumber}")]
        public async Task<Account> GetAccount(string accountNumber)
        {
            var result = await this.AccountQuerys.GetAccountByNumberAsync(accountNumber);
            return result;
        }
    }
}
