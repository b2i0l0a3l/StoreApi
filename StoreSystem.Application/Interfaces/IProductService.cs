using StoreSystem.Core.common;
using StoreSystem.Application.Contract.ProductContract.Req;
using StoreSystem.Application.Contract.ProductContract.Res;
using StoreSystem.Application.Contract.Common;

namespace StoreSystem.Application.Interfaces
{

    public interface IProductService
    {
        Task<GeneralResponse<int>> CreateAsync(ProductReq req);

        Task<GeneralResponse<bool?>> UpdateAsync(int id, ProductReq req);

        Task<GeneralResponse<bool?>> DeleteAsync(int id);

        Task<GeneralResponse<ProductRes?>> GetByIdAsync(int id);
        Task<GeneralResponse<PagedResult<ProductRes>>> GetAllAsync(GetProductReq req);

    }
}
