using System;
using MediatR;

namespace StoreSystem.Core.Events.InventoryEvent
{
    public class InventoryCreatedEvent : INotification
    {
        public int InventoryId { get; }
        public int StoreId { get; }
        public DateTime CreatedAt { get; }

        public InventoryCreatedEvent(int inventoryId, int storeId, DateTime createdAt)
        {
            InventoryId = inventoryId;
            StoreId = storeId;
            CreatedAt = createdAt;
        }
    }
}
