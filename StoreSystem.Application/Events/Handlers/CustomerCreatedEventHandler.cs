using System.Threading;
using System.Threading.Tasks;
using MediatR;
using StoreSystem.Core.Events.CustomerEvent;

namespace StoreSystem.Application.Events.Handlers
{
    /// <summary>
    /// Handle customer created event (placeholder for notifications/CRM sync).
    /// </summary>
    public class CustomerCreatedEventHandler : INotificationHandler<CustomerCreatedEvent>
    {
        public Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Placeholder: send welcome email, update CRM, etc.
            return Task.CompletedTask;
        }
    }
}
