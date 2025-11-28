using System;
using MediatR;

namespace StoreSystem.Core.Events.Audit
{
    public class EntityUpdatedEvent : INotification
    {
        public string EntityName { get; }
        public int EntityId { get; }
        public DateTime OccurredAt { get; }

        public EntityUpdatedEvent(string entityName, int entityId, DateTime occurredAt)
        {
            EntityName = entityName;
            EntityId = entityId;
            OccurredAt = occurredAt;
        }
    }
}
