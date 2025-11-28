using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StoreSystem.Core.Entities;

namespace StoreSystem.Core.Interfaces
{
    public interface IToken
    {
        Task<(bool,string)> AddTokenToDB(ApplicationUser user, string refreshToken);
        string GenerateAccessToken(IEnumerable<Claim> claims, bool isTemporary = false);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
        string? GenerateTemporaryToken(string UserId, int Minute);

    }
}