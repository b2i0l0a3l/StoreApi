using System;

namespace StoreSystem.Application.Contract.SupplierContract.Req
{
    public class GetSupplierReq
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Name { get; set; }
        public string? ContactName { get; set; }
        public string? Phone { get; set; }
    }
}
