using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounting.Api.Models
{
    public class ErrorModel
    {
        public string Type { get; }
        public string Message { get; }

        public ErrorModel(Exception ex)
        {
            this.Type = ex.GetType().Name;
            this.Message = ex.Message;
        }
    }
}
