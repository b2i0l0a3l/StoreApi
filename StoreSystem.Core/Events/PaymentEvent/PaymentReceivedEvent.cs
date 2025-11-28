using System;
using MediatR;

namespace StoreSystem.Core.Events.PaymentEvent
{
    public class PaymentReceivedEvent : INotification
    {
        public int PaymentId { get; }
        public decimal Amount { get; }
        public int CustomerId { get; }
        public DateTime Date { get; }

        public PaymentReceivedEvent(int paymentId, decimal amount, int customerId, DateTime date)
        {
            PaymentId = paymentId;
            Amount = amount;
            CustomerId = customerId;
            Date = date;
        }
    }
}
