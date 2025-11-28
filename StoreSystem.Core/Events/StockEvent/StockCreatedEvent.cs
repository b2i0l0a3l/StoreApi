using System;
using MediatR;

namespace StoreSystem.Core.Events.StockEvent
{
    public class StockCreatedEvent : INotification
    {
        public int StockId { get; }
        public int ProductId { get; }
        public int InventoryId { get; }
        public decimal Quantity { get; }
        public DateTime CreatedAt { get; }

        public StockCreatedEvent(int stockId, int productId, int inventoryId, decimal quantity, DateTime createdAt)
        {
            StockId = stockId;
            ProductId = productId;
            InventoryId = inventoryId;
            Quantity = quantity;
            CreatedAt = createdAt;
        }
    }
}
