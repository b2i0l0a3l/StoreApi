using System;
using MediatR;

namespace StoreSystem.Core.Events.InventoryEvent
{
    public class InventoryUpdatedEvent : INotification
    {
        public int InventoryId { get; }
        public DateTime UpdatedAt { get; }

        public InventoryUpdatedEvent(int inventoryId, DateTime updatedAt)
        {
            InventoryId = inventoryId;
            UpdatedAt = updatedAt;
        }
    }
}
