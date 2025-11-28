using System;

namespace StoreSystem.Application.Contract.PaymentContract.Res
{
    /// <summary>
    /// Response DTO for a payment.
    /// </summary>
    public class PaymentRes
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public DateTime Date { get; set; }
    }
}
