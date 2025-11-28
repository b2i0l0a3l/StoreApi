using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.ReturnContract.Req
{
    public class PurchaseReturnItemReq
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }

    public class PurchaseReturnReq
    {
        [Required]
        public int PurchaseId { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public IEnumerable<PurchaseReturnItemReq> Items { get; set; } = Array.Empty<PurchaseReturnItemReq>();
    }
}
