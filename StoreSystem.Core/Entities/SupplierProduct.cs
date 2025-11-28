using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Core.Entities;

namespace StoreSystem.Core.Entities
{
    public class SupplierProduct : baseEntity
    {
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public decimal CostPrice { get; set; }  
        public string? SupplierSku { get; set; }

        public Supplier Supplier { get; set; } = new Supplier();
        public Product Product { get; set; } = new Product();

    }
}