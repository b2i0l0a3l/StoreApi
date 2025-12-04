using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.SaleContract.Req;
using StoreSystem.Application.Contract.Common;
using StoreApi.Api.Attributes;
using StoreSystem.Core.Constants;
using StoreSystem.Application.Contract.SaleContract.Res;
using StoreSystem.Core.common;

namespace StoreApi.Api.Controllers
{
    [ApiController]
    [Route("api/Sale")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _service;
        public SaleController(ISaleService service) => _service = service;

        [HttpPost("AddSale")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [RequirePermission(PermissionCodes.SalesCreate)]
        public async Task<ActionResult<GeneralResponse<int>>> AddSale([FromBody] SaleReq req)
            => Ok(await _service.CreateSaleAsync(req));

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.SalesRead)]
        public async Task<ActionResult<GeneralResponse<SaleRes>>> GetById(int id)
            => Ok(await _service.GetByIdAsync(id));

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.SalesRead)]
        public async Task<ActionResult<GeneralResponse<PagedResult<SaleRes>>>> GetAll([FromQuery] GetSaleReq req)
            => Ok(await _service.GetAllAsync(req));
    }
}
