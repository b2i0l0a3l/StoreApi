using System;
using System.Collections.Generic;

namespace StoreSystem.Application.Contract.PurchaseContract.Res
{
    public class PurchaseRes
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<PurchaseItemRes> Items { get; set; } = Array.Empty<PurchaseItemRes>();
        public decimal Total => Items == null ? 0 : System.Linq.Enumerable.Sum(Items, i => i.LineTotal);
    }
}
