using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StoreSystem.Core.Entities;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Infrastructure.External.Auth
{
    public class UserClaimsService : IUserClaimsExtension
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserClaimsService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task AddDefaultClaimsAsync(ApplicationUser user, string role,int StoreId)
        {
            var claims = await _userManager.GetClaimsAsync(user);

                if (!claims.Any())
                {
                List<Claim> claimsList = new ()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, user.UserName! ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim("GoogleID", user.GoogleId ?? ""),
                    new Claim("StoreId",StoreId.ToString()??""),
                    new Claim(ClaimTypes.Role, role)
                };
                    await _userManager.AddClaimsAsync(user, claimsList);
                }
         }

        public async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            return claims.ToList();
        }
    }
}