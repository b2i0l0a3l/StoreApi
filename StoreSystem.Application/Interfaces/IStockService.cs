using System.Threading.Tasks;
using StoreSystem.Application.Contract.StockContract.req;
using StoreSystem.Application.Contract.StockContract.res;
using StoreSystem.Application.Contract.Common;

namespace StoreSystem.Application.Interfaces
{
    
    public interface IStockService
    {
        Task<GeneralResponse<int>> CreateStockAsync(StockReq req);
        Task<GeneralResponse<int>> IncreaseStockAsync(StockReq req);
        Task<GeneralResponse<int>> DecreaseStockAsync(StockReq req);
        Task<GeneralResponse<int>> AdjustStockAsync(StockReq req);
        Task<GeneralResponse<int>> GetCurrentStockAsync(StockReq req);
        Task<GeneralResponse<int>> GetLowStockProductsAsync(StockReq req);
    }
}
