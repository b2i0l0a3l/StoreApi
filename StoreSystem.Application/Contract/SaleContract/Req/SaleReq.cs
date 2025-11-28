using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.SaleContract.Req
{
    public class SaleReq
    {
        [Required]
        public int StoreId { get; set; }

        public int? CustomerId { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public IEnumerable<SaleItemReq> Items { get; set; } = Array.Empty<SaleItemReq>();
    }
}
