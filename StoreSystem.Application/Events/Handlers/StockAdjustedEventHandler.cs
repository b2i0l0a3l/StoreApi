using StoreSystem.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using StoreSystem.Core.Events.StockEvent;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Entities;
using BookingSystem.Core.enums;

namespace StoreSystem.Application.Events.Handlers
{
    public class StockAdjustedEventHandler : INotificationHandler<StockAdjustedEvent>
    {
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly IRepository<StoreSystem.Core.Entities.ProcessedEvent> _eventRepo;
        private readonly IUniteOfWork _uow;

        public StockAdjustedEventHandler(IRepository<Stock> stockRepo, IRepository<StockMovement> movementRepo, IRepository<StoreSystem.Core.Entities.ProcessedEvent> eventRepo, IUniteOfWork uow)
        {
            _stockRepo = stockRepo;
            _movementRepo = movementRepo;
            _eventRepo = eventRepo;
            _uow = uow;
        }

        public async Task Handle(StockAdjustedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var key = $"{nameof(StockAdjustedEvent)}:{notification.ProductId}:{notification.InventoryId}:{notification.AdjustedAt.Ticks}";
                var existing = await _eventRepo.FindAsync(e => e.EventType == nameof(StockAdjustedEvent) && e.AggregateId == key);
                if (existing != null) return;

                var stock = await _stockRepo.FindAsync(s => s.ProductId == notification.ProductId && s.InventoryId == notification.InventoryId);
                if (stock == null)
                {
                    stock = new Stock { ProductId = notification.ProductId, InventoryId = notification.InventoryId, Quantity = notification.NewQuantity, LastUpdated = notification.AdjustedAt };
                    await _stockRepo.AddAsync(stock);
                }
                else
                {
                    stock.Quantity = notification.NewQuantity;
                    stock.LastUpdated = notification.AdjustedAt;
                    await _stockRepo.UpdateAsync(stock);
                }

                var mv = new StockMovement
                {
                    ProductId = notification.ProductId,
                    InventoryId = notification.InventoryId,
                    Qty = (int)(notification.NewQuantity - notification.OldQuantity),
                    Type = MovementType.Adjustment,
                    ReferenceId = Guid.NewGuid(),
                    Date = notification.AdjustedAt,
                    Note = "Stock adjusted"
                };

                await _movementRepo.AddAsync(mv);
                await _eventRepo.AddAsync(new StoreSystem.Core.Entities.ProcessedEvent { EventType = nameof(StockAdjustedEvent), AggregateId = key, ProcessedAt = DateTime.UtcNow });
                await _uow.CompleteAsync();
            }
            catch
            {
            }
        }
    }
}
