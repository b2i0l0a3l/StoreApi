using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.Api.Attributes;
using StoreSystem.Core.Constants;
using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using StoreSystem.Application.Contract.Common;

namespace StoreApi.Api.Controllers
{
    [ApiController]
    [Route("api/Permission")]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ICurrentUserService _currentUserService;

        public PermissionController(IPermissionService permissionService, ICurrentUserService currentUserService)
        {
            _permissionService = permissionService;
            _currentUserService = currentUserService;
        }

        [HttpGet("GetAllPermissions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<GeneralResponse<IEnumerable<string>>> GetAllPermissions()
        {
            var permissions = PermissionCodes.GetAllPermissions();
            return Ok(GeneralResponse<IEnumerable<string>>.Success(permissions, "Success", 200));
        }

        [HttpGet("GetUserPermissions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GeneralResponse<IEnumerable<string>>>> GetUserPermissions()
        {
            if (_currentUserService.InventoryId == null)
                return Ok(GeneralResponse<IEnumerable<string>>.Success(Enumerable.Empty<string>(), "No store context", 200));

            // Note: We need StoreId, but ICurrentUserService might only have InventoryId depending on implementation.
            // Assuming StoreId is available in ICurrentUserService (I added it earlier).
            if (_currentUserService.StoreId == null)
                 return Ok(GeneralResponse<IEnumerable<string>>.Success(Enumerable.Empty<string>(), "No store context", 200));

            var permissions = await _permissionService.GetUserPermissionsAsync(_currentUserService.UserId!, _currentUserService.StoreId.Value);
            return Ok(GeneralResponse<IEnumerable<string>>.Success(permissions, "Success", 200));
        }

        [HttpGet("GetRolePermissions/{roleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.EmployeeRead)] // Or a specific permission for role management
        public async Task<ActionResult<GeneralResponse<IEnumerable<string>>>> GetRolePermissions(int roleId)
        {
            var permissions = await _permissionService.GetRolePermissionsAsync(roleId);
            return Ok(GeneralResponse<IEnumerable<string>>.Success(permissions, "Success", 200));
        }

        [HttpPost("AssignPermissionsToRole/{roleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission(PermissionCodes.EmployeeUpdate)] // Or a specific permission
        public async Task<ActionResult<GeneralResponse<bool>>> AssignPermissionsToRole(int roleId, [FromBody] IEnumerable<string> permissionCodes)
        {
            var result = await _permissionService.AssignPermissionsToRoleAsync(roleId, permissionCodes);
            if (result)
                return Ok(GeneralResponse<bool>.Success(true, "Permissions assigned successfully", 200));
            
            return Ok(GeneralResponse<bool>.Failure("Failed to assign permissions", 400));
        }
    }
}
