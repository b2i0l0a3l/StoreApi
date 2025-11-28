using System;

namespace StoreSystem.Application.Contract.SupplierProductContract.Res
{
    public class SupplierProductRes
    {
        public int Id { get; init; }
        public int SupplierId { get; init; }
        public int ProductId { get; init; }
        public decimal CostPrice { get; init; }
        public string? SupplierSku { get; init; }
    }
}
