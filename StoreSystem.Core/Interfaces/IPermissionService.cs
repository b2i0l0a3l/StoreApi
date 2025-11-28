using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreSystem.Core.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(string userId, int storeId, string permissionCode);
        Task<IEnumerable<string>> GetUserPermissionsAsync(string userId, int storeId);
        Task<bool> AssignPermissionsToRoleAsync(int roleId, IEnumerable<string> permissionCodes);
        Task<IEnumerable<string>> GetRolePermissionsAsync(int roleId);
        Task<bool> RemovePermissionFromRoleAsync(int roleId, string permissionCode);
    }
}
