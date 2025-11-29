using StoreSystem.Core.common;
using StoreSystem.Application.Contract.Categories.req;
using StoreSystem.Application.Contract.Categories.res;
using StoreSystem.Application.Contract.Common;

namespace StoreSystem.Application.Interfaces
{
    public interface ICategoryService 
    {
        Task<GeneralResponse<CategoryRes?>> GetByIdAsync(int id);
        Task<GeneralResponse<PagedResult<CategoryRes>>> GetAllForStoreAsync(GetCategoryReq entity);
        Task<GeneralResponse<int>> AddAsync(CategoryReq entity);
        Task<GeneralResponse<bool?>> Update(CategoryReq entity,int Id);
        Task<GeneralResponse<bool?>> DeleteAsync(int id);
    }
}