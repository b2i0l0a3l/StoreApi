using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StoreSystem.Core.Interfaces;

namespace StoreApi.Api.Attributes
{
 
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowTemporaryTokenAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var currentUserService = context.HttpContext.RequestServices
                .GetService(typeof(ICurrentUserService)) as ICurrentUserService;

            if (currentUserService == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }
            await Task.CompletedTask;
        }
    }
}
