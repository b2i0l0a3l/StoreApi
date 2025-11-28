using System;
using MediatR;

namespace StoreSystem.Core.Events.PurchaseEvent
{
    public class PurchaseReturnCreatedEvent : INotification
    {
        public int ReturnId { get; }
        public int PurchaseId { get; }
        public DateTime Date { get; }

        public PurchaseReturnCreatedEvent(int returnId, int purchaseId, DateTime date)
        {
            ReturnId = returnId;
            PurchaseId = purchaseId;
            Date = date;
        }
    }
}
