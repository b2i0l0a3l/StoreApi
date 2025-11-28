using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Core.enums;

namespace StoreSystem.Core.Entities
{
    public class PurchaseInvoice : baseEntity
    {
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; } = new Supplier();

        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaidAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DueAmount { get; set; }

        public InvoiceStatus Status { get; set; }
        
        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store Store { get; set; } = new Store();

        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}