using System;
using System.Collections.Generic;
using System.Text;

namespace Accounting.Services.Commands
{
    public class CreateAccountCommand
    {
        public string AccountNumber { get; set; }
        public string Owner { get; set; }
    }
}
