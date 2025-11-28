using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Infrastructure.External.Util
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;
        public CurrentUserService(IHttpContextAccessor Http)
        {
            _http = Http;
        }
        private bool IsAuth => _http.HttpContext!.User.Identity!.IsAuthenticated;
        public string? UserId => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        public int? StoreId
        {
            get
            {
                if (!IsAuth) return null; 
                
                string? Id = _http.HttpContext?.User?.FindFirstValue("StoreId");
                if (int.TryParse(Id, out int Value))
                    return Value;
                return null; 
            }
        }

        public int? InventoryId
        {
            get
            {
                if (!IsAuth) return null;

                string? Id = _http.HttpContext?.User?.FindFirstValue("InventoryId");
                if (int.TryParse(Id, out int Value))
                    return Value;
                return null;
            }
        }
        public bool IsAuthenticated => IsAuth;

        public bool IsTemporaryToken
        {
            get
            {
                if (!IsAuth) return false;
                
                string? isTemp = _http.HttpContext?.User?.FindFirstValue("Temporary");
                return isTemp == "True";
            }
        }

        public async Task<bool> HasPermissionAsync(string permissionCode)
        {
            if (!IsAuth || StoreId == null) return false;

            var permissions = await GetUserPermissionsAsync();
            return permissions.Contains(permissionCode);
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync()
        {
            if (!IsAuth || StoreId == null)
                return Enumerable.Empty<string>();

            // This will be injected via a separate service to avoid circular dependency
            // For now, return empty - will be implemented via IPermissionService
            return Enumerable.Empty<string>();
        }
    }
}