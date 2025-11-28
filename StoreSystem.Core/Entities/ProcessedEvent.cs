using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Core.Entities
{
    /// <summary>
    /// Simple marker entity used by application handlers to record processed notifications
    /// and provide a basic idempotency guard.
    /// </summary>
    public class ProcessedEvent : baseEntity
    {
        [MaxLength(200)]
        public string EventType { get; set; } = string.Empty;

        [MaxLength(200)]
        public string AggregateId { get; set; } = string.Empty;

        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
