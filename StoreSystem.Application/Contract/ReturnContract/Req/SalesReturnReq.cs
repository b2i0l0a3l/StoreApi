using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.ReturnContract.Req
{
    public class SalesReturnItemReq
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }

    public class SalesReturnReq
    {
        [Required]
        public int SaleId { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public IEnumerable<SalesReturnItemReq> Items { get; set; } = Array.Empty<SalesReturnItemReq>();
    }
}
