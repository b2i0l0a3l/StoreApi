using System;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.PaymentContract.Req
{
    /// <summary>
    /// Request DTO for recording a payment.
    /// </summary>
    public class PaymentReq
    {
        [Required]
        public decimal Amount { get; set; }

        public int? CustomerId { get; set; }

        public int? SupplierId { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
