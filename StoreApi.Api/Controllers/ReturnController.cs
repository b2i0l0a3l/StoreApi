using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.ReturnContract.Req;
using StoreSystem.Application.Contract.Common;
using Microsoft.AspNetCore.Authorization;

namespace StoreApi.Api.Controllers
{
    [ApiController]
    [Route("api/Return")]
    public class ReturnController : ControllerBase
    {
        private readonly IReturnService _service;
        public ReturnController(IReturnService service) => _service = service;

        [HttpPost("SalesReturn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeneralResponse<int>>> SalesReturn([FromBody] SalesReturnReq req)
            => Ok(await _service.CreateSalesReturnAsync(req));

        [HttpPost("PurchaseReturn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeneralResponse<int>>> PurchaseReturn([FromBody] PurchaseReturnReq req)
            => Ok(await _service.CreatePurchaseReturnAsync(req));
    }
}
