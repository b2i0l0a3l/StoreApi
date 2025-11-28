using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Core.enums;

namespace StoreSystem.Core.Entities
{
    public class SalesInvoice : baseEntity
    {
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = new Customer();

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

        public ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();
        
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}