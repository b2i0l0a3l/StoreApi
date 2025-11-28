using System;

namespace StoreSystem.Application.Contract.SupplierProductContract.Req
{
    public class SupplierProductReq
    {
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public decimal CostPrice { get; set; }
        public string? SupplierSku { get; set; }
    }
}
