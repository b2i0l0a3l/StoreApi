using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Application.Contract.InventoryContract.req
{
    public class InventoryReq
    {
        public DateTime CreateAt { get; set; }
        [Required]
        public int StoreId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
    }
}