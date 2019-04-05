using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Services.Commands
{
    public class MakeDepositCommand
    {
        public string AccountNumber { get; set; }
        public double Amount { get; set; }
    }
}
