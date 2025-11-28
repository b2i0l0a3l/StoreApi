using System;
using MediatR;

namespace StoreSystem.Core.Events.SupplierEvent
{
    public class SupplierCreatedEvent : INotification
    {
        public int SupplierId { get; }
        public string Name { get; }
        public DateTime CreatedAt { get; }

        public SupplierCreatedEvent(int supplierId, string name, DateTime createdAt)
        {
            SupplierId = supplierId;
            Name = name;
            CreatedAt = createdAt;
        }
    }
}
