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
    public class StockCreatedEventHandler : INotificationHandler<StockCreatedEvent>
    {
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly IRepository<StoreSystem.Core.Entities.ProcessedEvent> _eventRepo;
        private readonly IUniteOfWork _uow;

        public StockCreatedEventHandler(IRepository<Stock> stockRepo, IRepository<StockMovement> movementRepo, IRepository<StoreSystem.Core.Entities.ProcessedEvent> eventRepo, IUniteOfWork uow)
        {
            _stockRepo = stockRepo;
            _movementRepo = movementRepo;
            _eventRepo = eventRepo;
            _uow = uow;
        }

        public async Task Handle(StockCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var key = $"{nameof(StockCreatedEvent)}:{notification.StockId}";
                var existing = await _eventRepo.FindAsync(e => e.EventType == nameof(StockCreatedEvent) && e.AggregateId == notification.StockId.ToString());
                if (existing != null) return;

                var stock = await _stockRepo.FindAsync(s => s.Id == notification.StockId);
                if (stock == null)
                {
                    stock = new Stock
                    {
                        ProductId = notification.ProductId,
                        InventoryId = notification.InventoryId,
                        Quantity = notification.Quantity,
                        LastUpdated = notification.CreatedAt
                    };
                    await _stockRepo.AddAsync(stock);
                }
                else
                {
                    stock.Quantity = notification.Quantity;
                    stock.LastUpdated = notification.CreatedAt;
                    await _stockRepo.UpdateAsync(stock);
                }

                if (notification.Quantity > 0)
                {
                    var mv = new StockMovement
                    {
                        ProductId = notification.ProductId,
                        InventoryId = notification.InventoryId,
                        Qty = (int)notification.Quantity,
                        Type = MovementType.In,
                        ReferenceId = Guid.NewGuid(),
                        Date = notification.CreatedAt,
                        Note = "Stock created"
                    };
                    await _movementRepo.AddAsync(mv);
                }

                await _eventRepo.AddAsync(new StoreSystem.Core.Entities.ProcessedEvent { EventType = nameof(StockCreatedEvent), AggregateId = notification.StockId.ToString(), ProcessedAt = DateTime.UtcNow });
                await _uow.CompleteAsync();
            }
            catch
            {
            }
        }
    }
}
