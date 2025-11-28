using System;
using MediatR;

namespace StoreSystem.Core.Events.SaleEvent
{
    public class SaleCreatedEvent : INotification
    {
        public int SaleId { get; }
        public int StoreId { get; }
        public DateTime Date { get; }

        public SaleCreatedEvent(int saleId, int storeId, DateTime date)
        {
            SaleId = saleId;
            StoreId = storeId;
            Date = date;
        }
    }
}
