using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Core.Entities
{
    public class SalesItem : baseEntity
    {
        public int SalesInvoiceId { get; set; }

        [ForeignKey("SalesInvoiceId")]
        public SalesInvoice SalesInvoice { get; set; } = new SalesInvoice();

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = new Product();

        public int Qty { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SellPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Discount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }
    }
}