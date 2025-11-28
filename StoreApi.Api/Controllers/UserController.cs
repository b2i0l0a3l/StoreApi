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
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("Profile")]
            [Authorize]
            [ProducesResponseType(StatusCodes.Status200OK)]
            public async Task<ActionResult<GeneralResponse<ProfileRes>>> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");
            return Ok(await _userService.Profile(userId));
        }

        [HttpPut("Profile")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GeneralResponse<string?>>> UpdateProfile([FromBody] UpdateUserProfileReq req)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized("User not authenticated");

            return Ok(await _userService.UpdateProfile(UserId, req));
        }
         [HttpPut("ChangePassword")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GeneralResponse<string?>>> ChangePassword([FromBody] ChangePasswordReq req)
        {
            string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized("User not authenticated");
            return Ok(await _userService.ChangePassword(UserId,req));
        }
    }
}