using StoreSystem.Core.Interfaces;
using MediatR;
using StoreSystem.Core.Events.ProductEvent;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Events.Handlers
{
    public class ProductDeletedEventHandler : INotificationHandler<ProductDeletedEvent>
    {
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly IRepository<StoreSystem.Core.Entities.ProcessedEvent> _eventRepo;
        private readonly IUniteOfWork _uow;

        public ProductDeletedEventHandler(IRepository<Stock> stockRepo, IRepository<StockMovement> movementRepo, IRepository<StoreSystem.Core.Entities.ProcessedEvent> eventRepo, IUniteOfWork uow)
        {
            _stockRepo = stockRepo;
            _movementRepo = movementRepo;
            _eventRepo = eventRepo;
            _uow = uow;
        }

        public async Task Handle(ProductDeletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var key = $"{nameof(ProductDeletedEvent)}:{notification.ProductId}";
                var existing = await _eventRepo.FindAsync(e => e.EventType == nameof(ProductDeletedEvent) && e.AggregateId == notification.ProductId.ToString());
                if (existing != null) return;

                // remove stock movements
                var mv = await _movementRepo.FindAsync(m => m.ProductId == notification.ProductId);
                if (mv != null)
                {
                    _movementRepo.DeleteAsync(mv);
                }

                // remove stock entries
                var stock = await _stockRepo.FindAsync(s => s.ProductId == notification.ProductId);
                if (stock != null)
                {
                    _stockRepo.DeleteAsync(stock);
                }

                await _eventRepo.AddAsync(new StoreSystem.Core.Entities.ProcessedEvent { EventType = nameof(ProductDeletedEvent), AggregateId = notification.ProductId.ToString(), ProcessedAt = System.DateTime.UtcNow });
                await _uow.CompleteAsync();
            }
            catch
            {
            }
        }
    }
}
