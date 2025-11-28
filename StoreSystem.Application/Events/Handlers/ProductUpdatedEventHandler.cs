using StoreSystem.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using StoreSystem.Core.Events.ProductEvent;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Entities;
using BookingSystem.Core.enums;
using Microsoft.Extensions.Logging;

namespace StoreSystem.Application.Events.Handlers
{
    public class ProductUpdatedEventHandler : INotificationHandler<ProductUpdatedEvent>
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Inventory> _inventoryRepo;
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly IRepository<ProcessedEvent> _eventRepo;
        private readonly IUniteOfWork _uow;
        public ILogger<ProductUpdatedEventHandler> _logger;
        
        public ProductUpdatedEventHandler(IRepository<Product> productRepo,
            IRepository<Inventory> inventoryRepo,
            IRepository<Stock> stockRepo,
            IRepository<StockMovement> movementRepo,
            IRepository<ProcessedEvent> eventRepo,
            IUniteOfWork uow,
            ILogger<ProductUpdatedEventHandler> logger)
        {
            _productRepo = productRepo;
            _inventoryRepo = inventoryRepo;
            _stockRepo = stockRepo;
            _movementRepo = movementRepo;
            _eventRepo = eventRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task Handle(ProductUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var key = $"{nameof(ProductUpdatedEvent)}:{notification.ProductId}:{notification.UpdatedAt.Ticks}";
                var existing = await _eventRepo.FindAsync(e => e.EventType == nameof(ProductUpdatedEvent) && e.AggregateId == key);
                if (existing != null) return;

                var product = await _productRepo.FindAsync(p => p.Id == notification.ProductId);
                if (product == null) return;

                var inventory = await _inventoryRepo.FindAsync(i => i.StoreId == product.StoreId);
                if (inventory == null)
                {
                    inventory = new Inventory { StoreId = product.StoreId, Name = "Default Inventory", Location = "" };
                    await _inventoryRepo.AddAsync(inventory);
                    await _uow.Commit();
                }

                var stock = await _stockRepo.FindAsync(s => s.ProductId == product.Id && s.InventoryId == inventory.Id);
                if (stock == null)
                {
                    stock = new Stock { ProductId = product.Id, InventoryId = inventory.Id, Quantity = product.StockQuantity, LastUpdated = DateTime.UtcNow };
                    await _stockRepo.AddAsync(stock);

                    if (product.StockQuantity > 0)
                    {
                        await _movementRepo.AddAsync(new StockMovement
                        {
                            ProductId = product.Id,
                            InventoryId = inventory.Id,
                            Qty = product.StockQuantity,
                            Type = MovementType.In,
                            ReferenceId = null,
                            Date = DateTime.UtcNow,
                            Note = "Stock created on product update"
                        });
                    await _uow.Commit();

                    }
                }

                await _eventRepo.AddAsync(new ProcessedEvent { EventType = nameof(ProductUpdatedEvent), AggregateId = key, ProcessedAt = DateTime.UtcNow });
                await _uow.CompleteAsync();
                _logger.LogInformation("Successfully handled ProductUpdatedEvent for ProductId: {ProductId}", notification.ProductId);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error handling ProductUpdatedEvent for ProductId: {ProductId}", notification.ProductId);
                await _uow.Rollback();
            }
        }
    }
}
