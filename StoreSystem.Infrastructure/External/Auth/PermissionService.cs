using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StoreSystem.Core.Entities;
using StoreSystem.Core.Interfaces;
using StoreSystem.Infrastructure.Persistence;

namespace StoreSystem.Infrastructure.External.Auth
{
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const int CacheExpirationMinutes = 30;

        public PermissionService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<bool> UserHasPermissionAsync(string userId, int storeId, string permissionCode)
        {
            var permissions = await GetUserPermissionsAsync(userId, storeId);
            return permissions.Contains(permissionCode);
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(string userId, int storeId)
        {
            var cacheKey = $"user_permissions_{userId}_{storeId}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<string>? cachedPermissions) && cachedPermissions != null)
            {
                return cachedPermissions;
            }

            // Get employee record for this user in this store
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == userId && e.StoreId == storeId);

            if (employee == null)
            {
                // User is not an employee, check if they own the store
                var isOwner = await _context.Stores
                    .AsNoTracking()
                    .AnyAsync(s => s.Id == storeId && s.UserId == userId);

                if (isOwner)
                {
                    // Store owners have all permissions
                    return Core.Constants.PermissionCodes.GetAllPermissions();
                }

                return Enumerable.Empty<string>();
            }

            // Get permissions for the employee's role
            var permissions = await _context.RolePermissions
                .AsNoTracking()
                .Where(rp => rp.RoleId == employee.RoleId)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p.Code)
                .ToListAsync();

            // Cache the permissions
            _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(CacheExpirationMinutes));

            return permissions;
        }

        public async Task<bool> AssignPermissionsToRoleAsync(int roleId, IEnumerable<string> permissionCodes)
        {
            try
            {
                // Get permission IDs from codes
                var permissions = await _context.Permissions
                    .Where(p => permissionCodes.Contains(p.Code))
                    .ToListAsync();

                if (!permissions.Any())
                    return false;

                // Remove existing permissions for this role
                var existingRolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();

                _context.RolePermissions.RemoveRange(existingRolePermissions);

                // Add new permissions
                var newRolePermissions = permissions.Select(p => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = p.Id
                }).ToList();

                await _context.RolePermissions.AddRangeAsync(newRolePermissions);
                await _context.SaveChangesAsync();

                // Clear cache for all users with this role
                ClearRolePermissionsCache(roleId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetRolePermissionsAsync(int roleId)
        {
            var cacheKey = $"role_permissions_{roleId}";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<string>? cachedPermissions) && cachedPermissions != null)
            {
                return cachedPermissions;
            }

            var permissions = await _context.RolePermissions
                .AsNoTracking()
                .Where(rp => rp.RoleId == roleId)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p.Code)
                .ToListAsync();

            _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(CacheExpirationMinutes));

            return permissions;
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, string permissionCode)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Code == permissionCode);

                if (permission == null)
                    return false;

                var rolePermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

                if (rolePermission == null)
                    return false;

                _context.RolePermissions.Remove(rolePermission);
                await _context.SaveChangesAsync();

                // Clear cache
                ClearRolePermissionsCache(roleId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ClearRolePermissionsCache(int roleId)
        {
            // Clear role permissions cache
            _cache.Remove($"role_permissions_{roleId}");

            // Note: We can't easily clear all user caches for this role without tracking them
            // In a production system, you might want to use a distributed cache with tags
        }
    }
}
