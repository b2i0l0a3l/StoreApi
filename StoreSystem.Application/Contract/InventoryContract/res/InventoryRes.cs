using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Application.Contract.InventoryContract.res
{
    public class InventoryRes
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public int StoreId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }

    }
}