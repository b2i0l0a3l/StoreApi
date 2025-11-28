using StoreSystem.Core.common;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.PurchaseContract.Req;
using StoreSystem.Application.Contract.PurchaseContract.Res;

namespace StoreSystem.Application.Interfaces
{
    /// <summary>
    /// Service for handling purchases.
    /// </summary>
    public interface IPurchaseService
    {
        Task<GeneralResponse<int>> CreatePurchaseAsync(PurchaseReq req);
        Task<GeneralResponse<PurchaseRes?>> GetByIdAsync(int id);
        Task<GeneralResponse<PagedResult<PurchaseRes>>> GetAllAsync(int pageNumber, int pageSize);
        Task<GeneralResponse<int>> ReturnPurchaseAsync(StoreSystem.Application.Contract.ReturnContract.Req.PurchaseReturnReq req);
    }
}
