using StoreSystem.Core.common;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.StockMovementContract.req;
using StoreSystem.Application.Contract.StockMovementContract.res;
using StoreSystem.Application.Contract.Common;
using BookingSystem.Core.common;

namespace StoreSystem.Application.Interfaces
{

    public interface IStockMovementService
    {
        Task<GeneralResponse<int>> AddMovementAsync(StockMovementReq req);
        Task<GeneralResponse<PagedResult<StockMovementRes>>> GetMovementsByProductAsync(int productId, int pageNumber, int pageSize);
        Task<GeneralResponse<bool?>> UndoLastMovementAsync(int productId);
    }
}
