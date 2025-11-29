using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Core.enums;

namespace StoreSystem.Core.Entities
{
    public class Payment : baseEntity
    {
        public int InvoiceId { get; set; } 
    
        public PaymentType Type { get; set; } 

        // optional links to customer or supplier
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [MaxLength(50)]
        public string? Method { get; set; } 

        public DateTime Date { get; set; }
        
        [MaxLength(255)]
        public string? Note { get; set; }
    
        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store? Store { get; set; }
    }
}