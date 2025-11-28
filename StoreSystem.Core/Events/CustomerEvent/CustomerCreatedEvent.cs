using System;
using MediatR;

namespace StoreSystem.Core.Events.CustomerEvent
{
    public class CustomerCreatedEvent : INotification
    {
        public int CustomerId { get; }
        public string Name { get; }
        public DateTime CreatedAt { get; }

        public CustomerCreatedEvent(int customerId, string name, DateTime createdAt)
        {
            CustomerId = customerId;
            Name = name;
            CreatedAt = createdAt;
        }
    }
}

