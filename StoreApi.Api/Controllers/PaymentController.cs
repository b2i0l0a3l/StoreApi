using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.PaymentContract.Req;
using StoreSystem.Application.Contract.Common;
using Microsoft.AspNetCore.Authorization;

namespace StoreApi.Api.Controllers
{
    [ApiController]
    [Route("api/Payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;
        public PaymentController(IPaymentService service) => _service = service;

        [HttpPost("Record")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<GeneralResponse<int>>> Record([FromBody] PaymentReq req)
            => Ok(await _service.RecordPaymentAsync(req));

        [HttpGet("ByCustomer/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeneralResponse<object?>>> ByCustomer(int customerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
            => Ok(await _service.GetPaymentsByCustomerAsync(customerId, pageNumber, pageSize));

        [HttpGet("BySupplier/{supplierId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeneralResponse<object?>>> BySupplier(int supplierId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
            => Ok(await _service.GetPaymentsBySupplierAsync(supplierId, pageNumber, pageSize));
    }
}
