using StoreSystem.Core.common;
using StoreSystem.Application.Contract.ProductContract.Req;
using StoreSystem.Application.Contract.ProductContract.Res;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using StoreApi.Api.Attributes;
using StoreSystem.Core.Constants;

namespace StoreApi.Api.Controllers
{
    [ApiController]
    [Route("api/Product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductController(IProductService service) => _service= service;

        [HttpGet("GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.ProductRead)]
        public async Task<ActionResult<GeneralResponse<PagedResult<ProductRes>>>> GetAllProducts([FromQuery] GetProductReq Product)
        {
            return Ok(await _service.GetAllAsync(Product));
        }
        [HttpGet("{ProductId}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.ProductRead)]
        public async Task<ActionResult<GeneralResponse<ProductRes>>> GetProductById(int ProductId)
        {
            return Ok(await _service.GetByIdAsync(ProductId));
        }

        [HttpDelete("{ProductId}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.ProductDelete)]
        public async Task<ActionResult<GeneralResponse<bool?>>> DeleteProduct(int ProductId)
        {
            return Ok(await _service.DeleteAsync(ProductId));
        }
        [HttpPut("UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.ProductUpdate)]
        public async Task<ActionResult<GeneralResponse<bool?>>> UpdateProduct([FromBody] ProductReq Product, int ProductId)
        {
            return Ok(await _service.UpdateAsync(ProductId, Product));
        }

        [HttpPost("AddProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.ProductCreate)]
        public async Task<ActionResult<GeneralResponse<int>>> AddProduct([FromBody] ProductReq Product)
        {
            return Ok(await _service.CreateAsync(Product));
        }
        

    }
}