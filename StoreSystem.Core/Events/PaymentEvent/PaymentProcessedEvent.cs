using MediatR;

namespace StoreSystem.Core.Events.PaymentEvent
{
    /// <summary>
    /// Event fired when a payment is processed.
    /// </summary>
    public class PaymentProcessedEvent : INotification
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }

        public PaymentProcessedEvent(int paymentId, decimal amount, int? customerId, int? supplierId)
        {
            PaymentId = paymentId;
            Amount = amount;
            CustomerId = customerId;
            SupplierId = supplierId;
        }
    }
}
