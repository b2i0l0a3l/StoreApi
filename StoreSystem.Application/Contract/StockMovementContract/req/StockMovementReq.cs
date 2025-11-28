using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Core.enums;

namespace StoreSystem.Application.Contract.StockMovementContract.req
{
    public class StockMovementReq
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int InventoryId { get; set; }

        public int Qty { get; set; }

        public MovementType Type { get; set; }

        public Guid? ReferenceId { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
        
        [MaxLength(255)]
        public string? Note { get; set; }
    }
}