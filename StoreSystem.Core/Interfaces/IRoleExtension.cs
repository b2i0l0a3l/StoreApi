using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Core.Entities;

namespace StoreSystem.Core.Interfaces
{
    public interface IRoleExtension
    {
        Task<string?> EnsureUserInRoleAsync(ApplicationUser user, string role);

    }
}