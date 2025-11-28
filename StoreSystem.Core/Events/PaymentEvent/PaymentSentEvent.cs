using System;
using MediatR;

namespace StoreSystem.Core.Events.PaymentEvent
{
    public class PaymentSentEvent : INotification
    {
        public int PaymentId { get; }
        public decimal Amount { get; }
        public int SupplierId { get; }
        public DateTime Date { get; }

        public PaymentSentEvent(int paymentId, decimal amount, int supplierId, DateTime date)
        {
            PaymentId = paymentId;
            Amount = amount;
            SupplierId = supplierId;
            Date = date;
        }
    }
}
