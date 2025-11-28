using StoreSystem.Core.Interfaces;
using BookingSystem.Core.enums;
using StoreSystem.Core.Entities;
using MediatR;
using StoreSystem.Core.Events.ProductEvent;
using Microsoft.Extensions.Logging;

namespace StoreSystem.Application.Events.Handlers
{
    public class ProductCreatedEventHandler : INotificationHandler<ProductCreatedEvent>
    {
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly IRepository<Inventory> _inventoryRepo;
        private readonly IRepository<ProcessedEvent> _eventRepo;
        private readonly IUniteOfWork _uow;
        private ILogger<ProductCreatedEventHandler> _logger;

        public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger,IRepository<Stock> stockRepo, IRepository<StockMovement> movementRepo, IRepository<Inventory> inventoryRepo, IRepository<StoreSystem.Core.Entities.ProcessedEvent> eventRepo, IUniteOfWork uow)
        {
            _stockRepo = stockRepo;
            _movementRepo = movementRepo;
            _inventoryRepo = inventoryRepo;
            _eventRepo = eventRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                
                var existing = await _eventRepo.FindAsync(e => e.EventType == nameof(ProductCreatedEvent) && e.AggregateId == notification.Id.ToString());
                if (existing != null) return;

                var inventory = await _inventoryRepo.FindAsync(i => i.StoreId == notification.StoreId);
                
                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        StoreId = notification.StoreId,
                        Name = "Default Inventory",
                        Location = "",
                        CreateByUserId = notification.UserId,
                        UpdateByUserId = notification.UserId
                    };
                    await _inventoryRepo.AddAsync(inventory);
                    await _uow.Commit();
                }

                var stock = await _stockRepo.FindAsync(s => s.ProductId == notification.Id && s.InventoryId == inventory.Id);
                if (stock == null)
                {
                    stock = new Stock
                    {
                        ProductId = notification.Id,
                        InventoryId = inventory.Id,
                        Quantity = notification.StockQuantity,
                        LastUpdated = DateTime.UtcNow,
                        CreateByUserId = notification.UserId,
                        UpdateByUserId = notification.UserId
                    };
                    await _stockRepo.AddAsync(stock);
                    await _uow.Commit();

                }

                if (notification.StockQuantity > 0)
                {
                    var movement = new StockMovement
                    {
                        ProductId = notification.Id,
                        InventoryId = inventory.Id,
                        Qty = notification.StockQuantity,
                        Type = MovementType.In,
                        ReferenceId = null,
                        Date = DateTime.UtcNow,
                        Note = "Initial stock created on product creation",
                        CreateByUserId = notification.UserId,
                        UpdateByUserId = notification.UserId
                    };

                    await _movementRepo.AddAsync(movement);
                    await _uow.Commit();

                }

                await _eventRepo.AddAsync(new ProcessedEvent
                {
                    EventType = nameof(ProductCreatedEvent),
                    AggregateId = notification.Id.ToString(),
                    ProcessedAt = DateTime.UtcNow,
                    CreateByUserId = notification.UserId,
                    UpdateByUserId = notification.UserId
                });

                await _uow.CompleteAsync();
                _logger.LogInformation("Processed ProductCreatedEvent for ProductId: {ProductId}", notification.Id);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error processig ProductCreatedEvent for ProductId: {ProductId}", notification.Id);
                _logger.LogError("Error processig ProductCreatedEvent for ProductId: {Message}", ex.Message);
                await _uow.Rollback();
            }
        }
    }
}
