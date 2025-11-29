using System;
using System.Collections.Generic;
using BookingSystem.Core.enums;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Contract.SaleContract.Res
{
    public class SaleRes
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<SaleItemRes> Items { get; set; } = Array.Empty<SaleItemRes>();
        public InvoiceStatus StatusEnum { get; set; }
        public string Status => StatusEnum.ToString();
        public decimal Total => Items == null ? 0 : System.Linq.Enumerable.Sum(Items, i => i.LineTotal);
    }
}
