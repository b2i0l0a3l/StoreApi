using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.SaleContract.Req;
using StoreSystem.Application.Contract.Common;
using Microsoft.AspNetCore.Authorization;

namespace StoreApi.Api.Controllers
{
    [ApiController]
    [Route("api/Sale")]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _service;
        public SaleController(ISaleService service) => _service = service;

        [HttpPost("AddSale")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<GeneralResponse<int>>> AddSale([FromBody] SaleReq req)
            => Ok(await _service.CreateSaleAsync(req));

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeneralResponse<object?>>> GetById(int id)
            => Ok(await _service.GetByIdAsync(id));

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeneralResponse<object?>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
            => Ok(await _service.GetAllAsync(pageNumber, pageSize));
    }
}
