using System;
using MediatR;

namespace StoreSystem.Core.Events.StockMovementEvent
{
    public class StockMovementCreatedEvent : INotification
    {
        public int MovementId { get; }
        public int ProductId { get; }
        public int InventoryId { get; }
        public int Quantity { get; }
        public string Type { get; }
        public DateTime Date { get; }

        public StockMovementCreatedEvent(int movementId, int productId, int inventoryId, int quantity, string type, DateTime date)
        {
            MovementId = movementId;
            ProductId = productId;
            InventoryId = inventoryId;
            Quantity = quantity;
            Type = type;
            Date = date;
        }
    }
}
