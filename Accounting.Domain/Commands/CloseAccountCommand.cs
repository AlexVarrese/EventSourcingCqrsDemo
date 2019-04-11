using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Services.Commands
{
    public class CloseAccountCommand
    {
        public CloseAccountCommand(string accountNumber)
        {
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
        }

        public string AccountNumber { get; set; }
    }
}
