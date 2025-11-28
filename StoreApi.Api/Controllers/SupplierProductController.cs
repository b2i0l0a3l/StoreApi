using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.SupplierProductContract.Req;

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
        public async Task<IActionResult> Create(SupplierProductReq req)
        {
            var res = await _service.CreateAsync(req);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SupplierProductReq req)
        {
            var res = await _service.UpdateAsync(id, req);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _service.DeleteAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await _service.GetByIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("by-supplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(int supplierId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var res = await _service.GetAllBySupplierAsync(supplierId, page, pageSize);
            return StatusCode(res.StatusCode, res);
        }
    }
}
