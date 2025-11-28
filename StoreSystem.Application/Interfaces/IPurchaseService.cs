using StoreSystem.Core.common;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.PurchaseContract.Req;
using StoreSystem.Application.Contract.PurchaseContract.Res;
using StoreSystem.Application.Contract.ReturnContract.Req;

namespace StoreSystem.Application.Interfaces
{

    public interface IPurchaseService
    {
        Task<GeneralResponse<int>> CreatePurchaseAsync(PurchaseReq req);
        Task<GeneralResponse<PurchaseRes?>> GetByIdAsync(int id);
        Task<GeneralResponse<PagedResult<PurchaseRes>>> GetAllAsync(int pageNumber, int pageSize);
        Task<GeneralResponse<int>> ReturnPurchaseAsync(PurchaseReturnReq req);
    }
}
