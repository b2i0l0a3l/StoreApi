using System.Threading.Tasks;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.EmployeeContract.Req;
using StoreSystem.Application.Contract.EmployeeContract.Res;
using StoreSystem.Core.common;
using System;

namespace StoreSystem.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<GeneralResponse<int>> CreateEmployeeAsync(EmployeeReq req);
        Task<GeneralResponse<bool?>> UpdateEmployeeAsync(int id, EmployeeReq req);
        Task<GeneralResponse<bool?>> DeleteEmployeeAsync(int id);
        Task<GeneralResponse<EmployeeRes?>> GetByIdAsync(int id);
        Task<GeneralResponse<PagedResult<EmployeeRes>>> GetAllAsync(int pageNumber, int pageSize);
    }
}
