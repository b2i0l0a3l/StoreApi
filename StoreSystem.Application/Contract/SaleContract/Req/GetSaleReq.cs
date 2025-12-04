using System;

namespace StoreSystem.Application.Contract.SaleContract.Req
{
    public class GetSaleReq
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? CustomerId { get; set; }
    }
}
