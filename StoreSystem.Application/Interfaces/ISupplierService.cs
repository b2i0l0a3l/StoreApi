using StoreSystem.Core.common;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.SupplierContract.Req;
using StoreSystem.Application.Contract.SupplierContract.Res;

namespace StoreSystem.Application.Interfaces
{
    public interface ISupplierService
    {
        Task<GeneralResponse<int>> CreateSupplierAsync(SupplierReq req);
        Task<GeneralResponse<bool?>> UpdateSupplierAsync(int id, SupplierReq req);
        Task<GeneralResponse<bool?>> DeleteSupplierAsync(int id);
        Task<GeneralResponse<SupplierRes?>> GetByIdAsync(int id);
        Task<GeneralResponse<PagedResult<SupplierRes>>> GetAllAsync(int pageNumber, int pageSize);
    }
}
