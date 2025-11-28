using System;
using MediatR;

namespace StoreSystem.Core.Events.ProductEvent
{
    public class ProductUpdatedEvent : INotification
    {
        public int ProductId { get; }
        public DateTime UpdatedAt { get; }

        public ProductUpdatedEvent(int productId, DateTime updatedAt)
        {
            ProductId = productId;
            UpdatedAt = updatedAt;
        }
    }
}
