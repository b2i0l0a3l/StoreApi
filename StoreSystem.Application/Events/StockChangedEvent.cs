using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace StoreSystem.Application.Events
{
    /// <summary>
    /// Notification published when stock changes for a product.
    /// </summary>
    public class StockChangedEvent : INotification
    {
        public int ProductId { get; }
        [Required]
        [Range(1,int.MinValue, ErrorMessage = "InventoryId must be a non-negative integer.")]
        public int InventoryId { get; }
        public decimal Quantity { get; }
        public DateTime OccurredAt { get; }

        public StockChangedEvent(int productId, int inventoryId, decimal quantity)
        {
            ProductId = productId;
            InventoryId = inventoryId;
            Quantity = quantity;
            OccurredAt = DateTime.UtcNow;
        }
    }
}
