using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Services.Commands
{
    public class CloseAccoundCommand
    {
        public CloseAccoundCommand(string accountNumber)
        {
            AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
        }

        public string AccountNumber { get; set; }
    }
}
