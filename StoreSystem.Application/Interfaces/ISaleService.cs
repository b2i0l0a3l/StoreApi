using StoreSystem.Core.common;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.SaleContract.Req;
using StoreSystem.Application.Contract.SaleContract.Res;

namespace StoreSystem.Application.Interfaces
{
    /// <summary>
    /// Service for handling sales.
    /// </summary>
    public interface ISaleService
    {
        Task<GeneralResponse<int>> CreateSaleAsync(SaleReq req);
        Task<GeneralResponse<SaleRes?>> GetByIdAsync(int id);
        Task<GeneralResponse<PagedResult<SaleRes>>> GetAllAsync(int pageNumber, int pageSize);
        Task<GeneralResponse<int>> ReturnSaleAsync(StoreSystem.Application.Contract.ReturnContract.Req.SalesReturnReq req);
    }
}
