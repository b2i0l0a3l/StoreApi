using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.PurchaseContract.Req
{
    public class PurchaseReq
    {
        [Required]
        public int SupplierId { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public IEnumerable<PurchaseItemReq> Items { get; set; } = Array.Empty<PurchaseItemReq>();
    }
}
