using System;
using MediatR;

namespace StoreSystem.Core.Events.PurchaseEvent
{
    public class PurchaseCreatedEvent : INotification
    {
        public int PurchaseId { get; }
        public int SupplierId { get; }
        public DateTime Date { get; }

        public PurchaseCreatedEvent(int purchaseId, int supplierId, DateTime date)
        {
            PurchaseId = purchaseId;
            SupplierId = supplierId;
            Date = date;
        }
    }
}
