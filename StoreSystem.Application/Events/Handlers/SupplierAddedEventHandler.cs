using System.Threading;
using System.Threading.Tasks;
using MediatR;
using StoreSystem.Core.Events.SupplierEvent;
using StoreSystem.Core.Interfaces;
using StoreSystem.Application.Interfaces;

namespace StoreSystem.Application.Events.Handlers
{
    /// <summary>
    /// Handles supplier added events (extensible: notifications, integrations).
    /// </summary>
    public class SupplierAddedEventHandler : INotificationHandler<SupplierAddedEvent>
    {
        private readonly IUniteOfWork _uow;

        public SupplierAddedEventHandler(IUniteOfWork uow)
        {
            _uow = uow;
        }

        public Task Handle(SupplierAddedEvent notification, CancellationToken cancellationToken)
        {
            // For now: best-effort placeholder (e.g., send welcome email, sync to external system)
            return Task.CompletedTask;
        }
    }
}
