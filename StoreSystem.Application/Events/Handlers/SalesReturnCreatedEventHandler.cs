using StoreSystem.Core.Interfaces;
using System;
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
    public class SalesReturnCreatedEventHandler : INotificationHandler<SalesReturnCreatedEvent>
    {
        private readonly IRepository<SalesInvoice> _salesRepo;
        private readonly IRepository<Inventory> _inventoryRepo;
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly IRepository<StoreSystem.Core.Entities.ProcessedEvent> _eventRepo;
        private readonly IUniteOfWork _uow;
        private ILogger<SalesReturnCreatedEventHandler> _logger;

        public SalesReturnCreatedEventHandler(IRepository<SalesInvoice> salesRepo,
            IRepository<Inventory> inventoryRepo,
            IRepository<Stock> stockRepo,
            IRepository<StockMovement> movementRepo,
            IRepository<StoreSystem.Core.Entities.ProcessedEvent> eventRepo,
            IUniteOfWork uow,
            ILogger<SalesReturnCreatedEventHandler> logger)
        {
            _salesRepo = salesRepo;
            _inventoryRepo = inventoryRepo;
            _stockRepo = stockRepo;
            _movementRepo = movementRepo;
            _eventRepo = eventRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task Handle(SalesReturnCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var existing = await _eventRepo.FindAsync(e => e.EventType == nameof(SalesReturnCreatedEvent) && e.AggregateId == notification.SaleId.ToString());
                if (existing != null) return;

                await _eventRepo.AddAsync(new StoreSystem.Core.Entities.ProcessedEvent
                {
                    EventType = nameof(SalesReturnCreatedEvent),
                    AggregateId = notification.SaleId.ToString(),
                    ProcessedAt = DateTime.UtcNow
                });
                await _uow.Commit();
                var sale = await _salesRepo.FindAsync(s => s.Id == notification.SaleId);
                if (sale == null) return;

                var inventory = await _inventoryRepo.FindAsync(i => i.StoreId == sale.StoreId);
                if (inventory == null) return;

                foreach (var item in sale.SalesItems)
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
                    await _uow.Commit();

                    var mv = new StockMovement
                    {
                        ProductId = item.ProductId,
                        InventoryId = inventory.Id,
                        Qty = item.Qty,
                        Type = MovementType.In,
                        ReferenceId = Guid.NewGuid(),
                        Date = DateTime.UtcNow,
                        Note = $"Sales return for Sale #{notification.SaleId}"
                    };

                    await _movementRepo.AddAsync(mv);
                    await _uow.Commit();
                    _logger.LogInformation("Processed sales return for ProductId: {ProductId}, Qty: {Qty}", item.ProductId, item.Qty);

                }

                await _uow.CompleteAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError("Error processing SalesReturnCreatedEvent for SaleId: {SaleId}", notification.SaleId);
                _logger.LogError("Message: {Message}", ex.Message);
                await _uow.Rollback();
            }
        }
    }
}
