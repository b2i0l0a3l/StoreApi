using System;
using MediatR;

namespace StoreSystem.Core.Events.SaleEvent
{
    public class SalesReturnCreatedEvent : INotification
    {
        public int ReturnId { get; }
        public int SaleId { get; }
        public DateTime Date { get; }

        public SalesReturnCreatedEvent(int returnId, int saleId, DateTime date)
        {
            ReturnId = returnId;
            SaleId = saleId;
            Date = date;
        }
    }
}
