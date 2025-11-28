using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Core.enums;
using StoreSystem.Core.Entities;

namespace StoreSystem.Core.Entities
{
    public class StockMovement : baseEntity
    {
        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        [Required]
        public int InventoryId { get; set; }
        
        [ForeignKey("InventoryId")]
        public Inventory? Inventory { get; set; }

        public int Qty { get; set; }

        public MovementType Type { get; set; }

        public Guid? ReferenceId { get; set; }

        public DateTime Date { get; set; }
        
        [MaxLength(255)]
        public string? Note { get; set; }
    }
}