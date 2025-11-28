using StoreSystem.Core.common;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.ProductContract.Req;
using StoreSystem.Application.Contract.ProductContract.Res;
using StoreSystem.Application.Contract.Common;

namespace StoreSystem.Application.Interfaces
{
    /// <summary>
    /// Application service for managing products.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Create a new product.
        /// </summary>
        Task<GeneralResponse<int>> CreateAsync(ProductReq req);

        /// <summary>
        /// Update an existing product.
        /// </summary>
        Task<GeneralResponse<bool?>> UpdateAsync(int id, ProductReq req);

        /// <summary>
        /// Delete a product by id.
        /// </summary>
        Task<GeneralResponse<bool?>> DeleteAsync(int id);

        /// <summary>
        /// Get a product by id.
        /// </summary>
        Task<GeneralResponse<ProductRes?>> GetByIdAsync(int id);

        /// <summary>
        /// Get all products (paged).
        /// </summary>
        Task<GeneralResponse<PagedResult<ProductRes>>> GetAllAsync(GetProductReq req);

    }
}
