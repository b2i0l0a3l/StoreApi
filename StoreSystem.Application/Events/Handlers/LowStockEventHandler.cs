using StoreSystem.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using StoreSystem.Core.Events.StockEvent;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Events.Handlers
{
    public class LowStockEventHandler : INotificationHandler<LowStockEvent>
    {
        private readonly IRepository<Inventory> _inventoryRepo;
        private readonly IUniteOfWork _uow;

        public LowStockEventHandler(IRepository<Inventory> inventoryRepo, IUniteOfWork uow)
        {
            _inventoryRepo = inventoryRepo;
            _uow = uow;
        }

        public Task Handle(LowStockEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
