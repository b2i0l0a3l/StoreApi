using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.SaleContract.Req
{
    public class SaleReq
    {
        [Required]
        [Range(1,int.MaxValue,ErrorMessage ="Customer Id Must Greath than 0")]
        public int CustomerId { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        public IEnumerable<SaleItemReq> Items { get; set; } = Array.Empty<SaleItemReq>();
    }
}
