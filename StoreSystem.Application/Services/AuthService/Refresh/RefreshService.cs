using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.Token.req;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Http;
using StoreSystem.Core.Interfaces;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Services.AuthService.Refresh
{
    public class RefreshService : IRefresh
    {
        private readonly IRepository<TokenInfo> _TokenRepository;
        private readonly IToken _TokenService;
        public RefreshService(IToken Token,IRepository<TokenInfo>  token)
        {
            _TokenService = Token;
            _TokenRepository = token;
        }
        public async Task<GeneralResponse<TokenReq>> Refresh(TokenReq model)
        {
            if(model == null)
                return GeneralResponse<TokenReq>.Failure("Invalid token data", 400);
             try
            {
                var principal = _TokenService.GetPrincipalFromExpiredToken(model.AccessToken);
                var username = principal.Identity?.Name;

                var tokenInfo = await _TokenRepository.FindAsync(x=>x.Username == username!);

                if (tokenInfo == null
                    || tokenInfo.RefreshToken != model.RefreshToken 
                    || tokenInfo.Expiration <= DateTime.UtcNow)
                {
                    return GeneralResponse<TokenReq>.Failure("Invalid refresh token. Please login again.", 400);    
                }

                var newAccessToken = _TokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _TokenService.GenerateRefreshToken();

                tokenInfo.RefreshToken = newRefreshToken; 
                await _TokenRepository.UpdateAsync(tokenInfo);
                await _TokenRepository.SaveAsync();

                return GeneralResponse<TokenReq>.Success(new TokenReq
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }, "Token refreshed successfully", 200);
            }
            catch (Exception ex)
            {
                return GeneralResponse<TokenReq>.Failure(ex.Message, 500);
            }
        }
    }
}