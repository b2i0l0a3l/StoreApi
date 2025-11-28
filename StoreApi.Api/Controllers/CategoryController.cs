using StoreSystem.Core.common;
using System.Security.Claims;
using StoreSystem.Application.Contract.Categories.req;
using StoreSystem.Application.Contract.Categories.res;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StoreApi.Api.Attributes;
using StoreSystem.Core.Constants;

namespace CategoryApi.Api.Controllers
{
    [ApiController]
    [Route("api/Category")]
    // [Authorize] - Replaced by specific permissions
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpPost("GetAllCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequirePermission(PermissionCodes.CategoryRead)]
        public async Task<ActionResult<GeneralResponse<PagedResult<CategoryReq>>>> GetAllCategories(GetCategoryReq Category)
        => Ok(await _service.GetAllAsync(Category));
        
        [HttpPost("GetAllCategoriesForStore")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequirePermission(PermissionCodes.CategoryRead)]
        public async Task<ActionResult<GeneralResponse<PagedResult<CategoryReq>>>> GetAllCategoriesForStore(GetCategoryReq Category)
        {
            
            return Ok(await _service.GetAllForStoreAsync(Category));
        }


        [HttpGet("{CategoryId}", Name = "GetCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequirePermission(PermissionCodes.CategoryRead)]
        public async Task<ActionResult<GeneralResponse<CategoryRes>>> GetCategoryById(int CategoryId)
        {
            return Ok(await _service.GetByIdAsync(CategoryId));
        }

        [HttpDelete("{CategoryId}",Name ="DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequirePermission(PermissionCodes.CategoryDelete)]
        public async Task<ActionResult<GeneralResponse<bool?>>> DeleteCategory( int CategoryId)
        {
            return Ok(await _service.DeleteAsync(CategoryId));
        }

        [HttpPut("UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequirePermission(PermissionCodes.CategoryUpdate)]
        public async Task<ActionResult<GeneralResponse<bool?>>> UpdateCategory([FromBody] CategoryReq Category, int CategoryId)
        {

            return Ok(await _service.Update(Category, CategoryId));
        }

        [HttpPost("AddCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [RequirePermission(PermissionCodes.CategoryCreate)]
        public async Task<ActionResult<GeneralResponse<int>>> AddCategory([FromBody] CategoryReq Category)
        {
            return Ok(await _service.AddAsync( Category));
        }
    
    }
}