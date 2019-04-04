using AccountingApi.Domain;
using MediatR;

namespace Accounting.EventsProcessingFunction
{
    public class AccountEventRequest<TPayload> : IRequest<Account>
    {
        public TPayload Payload { get; }

        public AccountEventRequest(TPayload payload)
        {
            this.Payload = payload;
        }
    }
}
