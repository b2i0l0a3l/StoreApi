using StoreSystem.Core.common;
using System.Security.Claims;
using StoreSystem.Application.Contract.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Contract.StoreContract.req;
using StoreSystem.Application.Contract.StoreContract.res;
using StoreSystem.Application.Interfaces;

using StoreApi.Api.Attributes;
using StoreSystem.Core.Constants;

namespace StoreApi.Api.Controllers
{
    [ApiController]
    [Route("api/Store")]
    // [Authorize] - Replaced by specific permissions
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _store;
        public StoreController(IStoreService store) => _store = store;

        [HttpPost("GetAllStore")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequirePermission(PermissionCodes.StoreRead)]
        public async Task<ActionResult<GeneralResponse<PagedResult<StoreRes>>>> GetAllStore(GetStoreReq store)
        => Ok(await _store.GetAllAsync(store));
        
        [HttpGet("GetStoreByJwt")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.StoreRead)]
        public async Task<ActionResult<GeneralResponse<StoreRes?>>> GetStoreByJwt()
        {
            return Ok(await _store.GetStoreByJwtAsync());
        }

        [HttpGet("{StoreId}", Name = "GetStoreById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.StoreRead)]
        public async Task<ActionResult<GeneralResponse<StoreRes>>> GetStoreById(int StoreId)
        => Ok(await _store.GetByIdAsync(StoreId));

        
        [HttpDelete("{StoreId}", Name = "DeleteStore")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.StoreDelete)]
        public async Task<ActionResult<GeneralResponse<bool?>>> DeleteStore( int StoreId)
        => Ok(await _store.DeleteAsync(StoreId));

        [HttpPut("UpdateStore")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.StoreUpdate)]
        public async Task<ActionResult<GeneralResponse<bool?>>> UpdateStore([FromBody] StoreReq store, int StoreId)
        => Ok(await _store.Update(store, StoreId));
    
        [HttpPost("AddStore")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowTemporaryToken]
        public async Task<ActionResult<GeneralResponse<StoreRes>>> AddStore([FromBody] StoreReq store)
        {
            return Ok(await _store.AddAsync(store));
        }
        
    }
}