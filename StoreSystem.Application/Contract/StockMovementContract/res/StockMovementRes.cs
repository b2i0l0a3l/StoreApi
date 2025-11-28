using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Core.enums;

namespace StoreSystem.Application.Contract.StockMovementContract.res
{
    public class StockMovementRes
    {
        public int Id { get; set; }
        public DateTime CreateAt{ get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int InventoryId { get; set; }
        public int Qty { get; set; }

        public MovementType Type { get; set; }

        public Guid? ReferenceId { get; set; }

        public DateTime Date { get; set; }
        
        public string? Note { get; set; }
    }
}