using StoreSystem.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookingSystem.Core.enums;
using StoreSystem.Core.Entities;
using MediatR;
using StoreSystem.Core.Events.SaleEvent;
using StoreSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace StoreSystem.Application.Events.Handlers
{
    public class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
    {
        private readonly IRepository<SalesInvoice> _salesRepo;
        private readonly IRepository<Inventory> _inventoryRepo;
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly IRepository<StoreSystem.Core.Entities.ProcessedEvent> _eventRepo;
        private readonly IUniteOfWork _uow;
        private ILogger<SaleCreatedEventHandler> _logger;

        public SaleCreatedEventHandler(IRepository<SalesInvoice> salesRepo,
            IRepository<Inventory> inventoryRepo,
            IRepository<Stock> stockRepo,
            IRepository<StockMovement> movementRepo,
            IRepository<StoreSystem.Core.Entities.ProcessedEvent> eventRepo,
            IUniteOfWork uow,
            ILogger<SaleCreatedEventHandler> logger)
        {
            _salesRepo = salesRepo;
            _inventoryRepo = inventoryRepo;
            _stockRepo = stockRepo;
            _movementRepo = movementRepo;
            _eventRepo = eventRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var key = $"{nameof(SaleCreatedEvent)}:{notification.SaleId}";
                var existing = await _eventRepo.FindAsync(e => e.EventType == nameof(SaleCreatedEvent) && e.AggregateId == notification.SaleId.ToString());
                if (existing != null) return;

                await _eventRepo.AddAsync(new StoreSystem.Core.Entities.ProcessedEvent
                {
                    EventType = nameof(SaleCreatedEvent),
                    AggregateId = notification.SaleId.ToString(),
                    ProcessedAt = DateTime.UtcNow
                });
                await _uow.Commit();

                var invoice = await _salesRepo.FindAsync(i => i.Id == notification.SaleId);
                if (invoice == null) return;

                var inventory = await _inventoryRepo.FindAsync(i => i.StoreId == notification.StoreId);
                if (inventory == null) return;

                foreach (var item in invoice.SalesItems)
                {
                    var stock = await _stockRepo.FindAsync(s => s.ProductId == item.ProductId && s.InventoryId == inventory.Id);
                    if (stock == null)
                    {
                        stock = new Stock
                        {
                            ProductId = item.ProductId,
                            InventoryId = inventory.Id,
                            Quantity = 0,
                            LastUpdated = DateTime.UtcNow
                        };
                        await _stockRepo.AddAsync(stock);
                        await _uow.Commit();

                    }

                    stock.Quantity -= item.Qty;
                    stock.LastUpdated = DateTime.UtcNow;
                    await _stockRepo.UpdateAsync(stock);
                    await _uow.Commit();

                    // create movement (out)
                    var mv = new StockMovement
                    {
                        ProductId = item.ProductId,
                        InventoryId = inventory.Id,
                        Qty = item.Qty,
                        Type = MovementType.Out,
                        ReferenceId = Guid.NewGuid(),
                        Date = DateTime.UtcNow,
                        Note = $"Sale #{notification.SaleId}"
                    };

                    await _movementRepo.AddAsync(mv);
                     await _uow.Commit();

                }

                await _uow.CompleteAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError("Error processing SaleCreatedEvent for SaleId: {SaleId}", notification.SaleId);
                _logger.LogError("Message: {Message}", ex.Message);
                await _uow.Rollback();
            }
        }
    }
}
