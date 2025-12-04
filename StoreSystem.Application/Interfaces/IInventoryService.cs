using StoreSystem.Core.common;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.InventoryContract.res;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.InventoryContract.req;

namespace StoreSystem.Application.Interfaces
{
    /// <summary>
    /// Service to manage inventories (warehouses).
    /// </summary>
    public interface IInventoryService
    {
        Task<GeneralResponse<int>> CreateInventoryAsync(InventoryReq req);
        Task<GeneralResponse<bool?>> UpdateInventoryAsync(int id, InventoryReq req);
        Task<GeneralResponse<bool?>> DeleteInventoryAsync(int id);
        Task<GeneralResponse<InventoryRes?>> GetByIdAsync(int id);
        Task<GeneralResponse<PagedResult<InventoryRes>>> GetAllAsync(GetAllInventoryReq req);
    }
}
