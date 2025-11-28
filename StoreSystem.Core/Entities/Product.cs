using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Core.Interfaces;
using StoreSystem.Core.Entities;
using StoreSystem.Core.enums;

namespace StoreSystem.Core.Entities
{
    public class Product : baseEntity 
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SKU { get; set; } 

        [MaxLength(50)]
        public string? Barcode { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CostPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SellPrice { get; set; }
        public UnitType Unit { get; set; }           
        public int StockQuantity { get; set; }
        public DateTime UpdateAt { get; set; }
        public int CategoryId { get; set; }


        [ForeignKey("CategoryId")]
        public Category? Category { get; set; } 
        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store? Store { get; set; }

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
        public ICollection<SupplierProduct> SupplierProducts { get; set; } = new List<SupplierProduct>();
        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
        public ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
        
    }
}