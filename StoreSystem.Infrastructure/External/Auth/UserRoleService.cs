using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StoreSystem.Core.Entities;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Infrastructure.External.Auth
{
    public class UserRoleService : IRoleExtension
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRoleService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string?> EnsureUserInRoleAsync(ApplicationUser user, string role)
        {
            if (!await _userManager.IsInRoleAsync(user, role))
            {
                var result = await _userManager.AddToRoleAsync(user, role);
                if (!result.Succeeded)
                    return string.Join(", ", result.Errors.Select(e => e.Description));
            }
            return null;
        }
        public async Task<List<string>?> GetUserRoles(string UserId, int StoreId)
        {
            ApplicationUser? User = await _userManager.FindByIdAsync(UserId);
            if (User == null) return null;
            var RolesResult = await _userManager.GetRolesAsync(User);
            return RolesResult.ToList();
        }
    }
}