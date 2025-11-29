using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.EmployeeContract.Req;
using StoreApi.Api.Attributes;
using StoreSystem.Core.Constants;
using Microsoft.AspNetCore.Authorization;

namespace BookingApi.Api.Controllers
{
    [ApiController]
    [Route("api/employees")]
    [Authorize("Admin,User")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Add(EmployeeReq req)
        {
            var res = await _service.CreateEmployeeAsync(req);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{id}",Name ="Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, EmployeeReq req)
        {
            var res = await _service.UpdateEmployeeAsync(id, req);
            return StatusCode(res.StatusCode, res);
        }

        [HttpDelete("{id}", Name = "Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> Delete(int id)
        {
            var res = await _service.DeleteEmployeeAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> Get(int id)
        {
            var res = await _service.GetByIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _service.GetAllAsync(page, pageSize));
        
    }
}
