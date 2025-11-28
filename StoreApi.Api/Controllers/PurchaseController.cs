using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.PurchaseContract.Req;
using StoreSystem.Application.Contract.Common;
using Microsoft.AspNetCore.Authorization;
using StoreApi.Api.Attributes;
using StoreSystem.Core.Constants;

namespace StoreApi.Api.Controllers
{
    [ApiController]
    [Route("api/Purchase")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _service;
        public PurchaseController(IPurchaseService service) => _service = service;

        [HttpPost("AddPurchase")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [RequirePermission(PermissionCodes.PurchaseCreate)]
        
        public async Task<ActionResult<GeneralResponse<int>>> AddPurchase([FromBody] PurchaseReq req)
            => Ok(await _service.CreatePurchaseAsync(req));

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.PurchaseRead)]

        public async Task<ActionResult<GeneralResponse<object?>>> GetById(int id)
            => Ok(await _service.GetByIdAsync(id));

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.PurchaseRead)]
        public async Task<ActionResult<GeneralResponse<object?>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
            => Ok(await _service.GetAllAsync(pageNumber, pageSize));
    }
}
