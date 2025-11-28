using System.Threading.Tasks;
using StoreSystem.Core.common;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.SupplierProductContract.Req;
using StoreSystem.Application.Contract.SupplierProductContract.Res;

namespace StoreSystem.Application.Interfaces
{
    public interface ISupplierProductService
    {
        Task<GeneralResponse<int>> CreateAsync(SupplierProductReq req);
        Task<GeneralResponse<bool?>> UpdateAsync(int id, SupplierProductReq req);
        Task<GeneralResponse<bool?>> DeleteAsync(int id);
        Task<GeneralResponse<SupplierProductRes?>> GetByIdAsync(int id);
        Task<GeneralResponse<PagedResult<SupplierProductRes>>> GetAllBySupplierAsync(int supplierId, int page, int pageSize);
    }
}
