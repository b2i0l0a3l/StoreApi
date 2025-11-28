using StoreSystem.Application.Contract.Common;

namespace StoreSystem.Application.Interfaces
{

    public interface IReturnService
    {
        Task<GeneralResponse<int>> CreateSalesReturnAsync(StoreSystem.Application.Contract.ReturnContract.Req.SalesReturnReq req);
        Task<GeneralResponse<int>> CreatePurchaseReturnAsync(StoreSystem.Application.Contract.ReturnContract.Req.PurchaseReturnReq req);
    }
}
