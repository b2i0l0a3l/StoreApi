using System;
using MediatR;

namespace StoreSystem.Core.Events.Audit
{
    public class EntityDeletedEvent : INotification
    {
        public string EntityName { get; }
        public int EntityId { get; }
        public DateTime OccurredAt { get; }

        public EntityDeletedEvent(string entityName, int entityId, DateTime occurredAt)
        {
            EntityName = entityName;
            EntityId = entityId;
            OccurredAt = occurredAt;
        }
    }
}
