using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Contract.User.req;
using StoreSystem.Application.Contract.User.res;
using StoreSystem.Application.Interfaces;

namespace StoreApi.Api.Controllers
{
    [ApiController]
    [Route("api/Profile")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
            [HttpGet("Profile")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            public async Task<ActionResult<GeneralResponse<ProfileRes>>> Profile()
            => Ok(await _userService.Profile());

        [HttpPut("Profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeneralResponse<string?>>> UpdateProfile([FromBody] UpdateUserProfileReq req)
          => Ok(await _userService.UpdateProfile( req));
        
        
        [HttpPut("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeneralResponse<string?>>> ChangePassword([FromBody] ChangePasswordReq req)
            => Ok(await _userService.ChangePassword(req));
    }
}