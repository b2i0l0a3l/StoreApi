using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StoreSystem.Core.Interfaces;

namespace StoreApi.Api.Attributes
{
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _permissionCodes;

        public RequirePermissionAttribute(params string[] permissionCodes)
        {
            _permissionCodes = permissionCodes ?? throw new ArgumentNullException(nameof(permissionCodes));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get current user service
            var currentUserService = context.HttpContext.RequestServices
                .GetService(typeof(ICurrentUserService)) as ICurrentUserService;

            if (currentUserService == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            if (currentUserService.IsTemporaryToken)
            {
                context.Result = new ObjectResult(new { message = "Please create a store first to access this resource" })
                {
                    StatusCode = 403
                };
                return;
            }

            bool hasPermission = false;
            foreach (var permissionCode in _permissionCodes)
            {
                if (await currentUserService.HasPermissionAsync(permissionCode))
                {
                    hasPermission = true;
                    break;
                }
            }

            if (!hasPermission)
            {
                context.Result = new ObjectResult(new { message = "You don't have permission to access this resource" })
                {
                    StatusCode = 403
                };
            }
        }
    }
}
