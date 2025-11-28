using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Core.Interfaces
{
    public interface ICurrentUserService
    {
        public string? UserId { get;}
        public int? StoreId { get;}
        public int? InventoryId { get; }
        public bool IsAuthenticated { get; }
        public bool IsTemporaryToken { get; }
        
        Task<bool> HasPermissionAsync(string permissionCode);
        Task<IEnumerable<string>> GetUserPermissionsAsync();
    }
}