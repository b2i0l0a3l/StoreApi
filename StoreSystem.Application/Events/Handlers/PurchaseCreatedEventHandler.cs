using StoreSystem.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using BookingSystem.Core.enums;
using StoreSystem.Core.Entities;
using MediatR;
using StoreSystem.Core.Events.PurchaseEvent;
using StoreSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace StoreSystem.Application.Events.Handlers
{
    public class PurchaseCreatedEventHandler : INotificationHandler<PurchaseCreatedEvent>
    {
        private readonly IRepository<PurchaseInvoice> _purchaseRepo;
        private readonly IRepository<Inventory> _inventoryRepo;
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly IRepository<StoreSystem.Core.Entities.ProcessedEvent> _eventRepo;
        private readonly IUniteOfWork _uow;
        private ILogger<PurchaseCreatedEventHandler> _logger;

        public PurchaseCreatedEventHandler(IRepository<PurchaseInvoice> purchaseRepo,
            IRepository<Inventory> inventoryRepo,
            IRepository<Stock> stockRepo,
            IRepository<StockMovement> movementRepo,
            IRepository<StoreSystem.Core.Entities.ProcessedEvent> eventRepo,
            IUniteOfWork uow,ILogger<PurchaseCreatedEventHandler> logger)
        {
            _purchaseRepo = purchaseRepo;
            _inventoryRepo = inventoryRepo;
            _stockRepo = stockRepo;
            _movementRepo = movementRepo;
            _eventRepo = eventRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task Handle(PurchaseCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var existing = await _eventRepo.FindAsync(e => e.EventType == nameof(PurchaseCreatedEvent) && e.AggregateId == notification.PurchaseId.ToString());
                if (existing != null) return;

                await _eventRepo.AddAsync(new StoreSystem.Core.Entities.ProcessedEvent
                {
                    EventType = nameof(PurchaseCreatedEvent),
                    AggregateId = notification.PurchaseId.ToString(),
                    ProcessedAt = DateTime.UtcNow
                });
                var purchase = await _purchaseRepo.FindAsync(p => p.Id == notification.PurchaseId);
                if (purchase == null) return;

                var inventory = await _inventoryRepo.FindAsync(i => i.StoreId == purchase.StoreId);
                if (inventory == null) return;

                foreach (var item in purchase.PurchaseItems)
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
                    }

                    stock.Quantity += item.Qty;
                    stock.LastUpdated = DateTime.UtcNow;
                    await _stockRepo.UpdateAsync(stock);

                    var mv = new StockMovement
                    {
                        ProductId = item.ProductId,
                        InventoryId = inventory.Id,
                        Qty = item.Qty,
                        Type = MovementType.In,
                        ReferenceId = Guid.NewGuid(),
                        Date = DateTime.UtcNow,
                        Note = $"Purchase #{notification.PurchaseId}"
                    };

                    await _movementRepo.AddAsync(mv);
                }
                await _uow.CompleteAsync();
                _logger.LogInformation("Processed PurchaseCreatedEvent for PurchaseId: {PurchaseId}", notification.PurchaseId);
            }
            catch
            {
                _logger.LogError("Error processing PurchaseCreatedEvent for PurchaseId: {PurchaseId}", notification.PurchaseId);
            }
        }
    }
}
