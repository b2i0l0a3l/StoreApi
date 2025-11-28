using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Core.Entities;

namespace StoreSystem.Core.Entities
{
    public class Inventory : baseEntity
    {
        [Required]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public Store? Store { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
        public ICollection<Category> categories { get; set; } = new List<Category>();
        

    }
}