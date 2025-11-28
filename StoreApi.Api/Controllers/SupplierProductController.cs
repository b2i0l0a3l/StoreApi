using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.SupplierProductContract.Req;
using StoreSystem.Application.Contract.Common;
using StoreApi.Api.Attributes;
using StoreSystem.Core.Constants;

namespace BookingApi.Api.Controllers
{
    [ApiController]
    [Route("api/supplier-products")]
    public class SupplierProductController : ControllerBase
    {
        private readonly ISupplierProductService _service;

        public SupplierProductController(ISupplierProductService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [RequirePermission(PermissionCodes)]

        public async Task<ActionResult<GeneralResponse<int>>> Create(SupplierProductReq req)
            => Ok(await _service.CreateAsync(req));

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeneralResponse<bool?>>> Update(int id, SupplierProductReq req)
        => Ok(await _service.UpdateAsync(id, req));
        

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
            => Ok(await _service.DeleteAsync(id));

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id)
            => Ok(await _service.GetByIdAsync(id));

        [HttpGet("by-supplier/{supplierId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBySupplier(int supplierId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => Ok(await _service.GetAllBySupplierAsync(supplierId, page, pageSize));
    }
}
