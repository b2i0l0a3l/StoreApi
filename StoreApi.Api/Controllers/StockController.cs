using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.StockContract.req;
using StoreSystem.Application.Contract.StockContract.res;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using StoreApi.Api.Attributes;
using StoreSystem.Core.Constants;

namespace StoreApi.Api.Controllers
{
    [ApiController]
    [Route("api/Stock")]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly IStockService _service;
        public StockController(IStockService service) => _service = service;

        [HttpPost("GetCurrent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.StockRead)]
        public async Task<ActionResult<GeneralResponse<int>>> GetCurrent([FromBody] StockReq req)
            => Ok(await _service.GetCurrentStockAsync(req));

        [HttpPost("Increase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.StockUpdate)]
        public async Task<ActionResult<GeneralResponse<int>>> Increase([FromBody] StockReq req)
            => Ok(await _service.IncreaseStockAsync(req));

        [HttpPost("Decrease")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.StockUpdate)]
        public async Task<ActionResult<GeneralResponse<int>>> Decrease([FromBody] StockReq req)
            => Ok(await _service.DecreaseStockAsync(req));

        [HttpPost("Adjust")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.StockUpdate)]
        public async Task<ActionResult<GeneralResponse<int>>> Adjust([FromBody] StockReq req)
            => Ok(await _service.AdjustStockAsync(req));
    }
}