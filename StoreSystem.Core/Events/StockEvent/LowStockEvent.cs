using System;
using MediatR;

namespace StoreSystem.Core.Events.StockEvent
{
    public class LowStockEvent : INotification
    {
        public int ProductId { get; }
        public int InventoryId { get; }
        public decimal CurrentQuantity { get; }
        public decimal Threshold { get; }
        public DateTime OccurredAt { get; }

        public LowStockEvent(int productId, int inventoryId, decimal currentQuantity, decimal threshold, DateTime occurredAt)
        {
            ProductId = productId;
            InventoryId = inventoryId;
            CurrentQuantity = currentQuantity;
            Threshold = threshold;
            OccurredAt = occurredAt;
        }
    }
}
