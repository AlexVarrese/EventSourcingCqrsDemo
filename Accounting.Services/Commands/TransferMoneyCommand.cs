using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Services.Commands
{
    public class TransferMoneyCommand
    {
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public double Amount { get; set; }
    }
}
