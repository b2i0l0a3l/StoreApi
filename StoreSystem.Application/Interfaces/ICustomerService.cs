using StoreSystem.Core.common;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.CustomerContract.Req;
using StoreSystem.Application.Contract.CustomerContract.Res;

namespace StoreSystem.Application.Interfaces
{
    /// <summary>
    /// Service interface for customer operations.
    /// </summary>
    public interface ICustomerService
    {
        Task<GeneralResponse<int>> CreateCustomerAsync(CustomerReq req);
        Task<GeneralResponse<bool?>> UpdateCustomerAsync(int id, CustomerReq req);
        Task<GeneralResponse<bool?>> DeleteCustomerAsync(int id);
        Task<GeneralResponse<CustomerRes?>> GetByIdAsync(int id);
        Task<GeneralResponse<PagedResult<CustomerRes>>> GetAllAsync(GetCustomerReq req);
    }
}
