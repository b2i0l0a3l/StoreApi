using System;
using MediatR;

namespace StoreSystem.Core.Events.StockEvent
{
    public class StockAdjustedEvent : INotification
    {
        public int ProductId { get; }
        public int InventoryId { get; }
        public decimal OldQuantity { get; }
        public decimal NewQuantity { get; }
        public DateTime AdjustedAt { get; }

        public StockAdjustedEvent(int productId, int inventoryId, decimal oldQuantity, decimal newQuantity, DateTime adjustedAt)
        {
            ProductId = productId;
            InventoryId = inventoryId;
            OldQuantity = oldQuantity;
            NewQuantity = newQuantity;
            AdjustedAt = adjustedAt;
        }
    }
}
