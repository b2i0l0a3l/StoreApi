using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StoreSystem.Core.Entities;

namespace StoreSystem.Core.Interfaces
{
    public interface IUserClaimsExtension
    {
        
        Task AddDefaultClaimsAsync(ApplicationUser user, string role,int StoreId);
        Task<List<Claim>> GetClaimsAsync(ApplicationUser user);

    }
}