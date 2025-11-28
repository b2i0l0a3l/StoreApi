using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Core.Entities
{
    public class Store : baseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(255)]
        public string? Address { get; set; }
        [MaxLength(20)]
        public string? Phone { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

    
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<StockMovement> stockMovements { get; set; } = new List<StockMovement>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<StoreEmployee> Employees { get; set; } = new List<StoreEmployee>();
        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
        public ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
    }


}