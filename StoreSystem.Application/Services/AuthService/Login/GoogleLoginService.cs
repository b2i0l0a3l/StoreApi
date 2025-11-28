using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookingSystem.Core.common;
using Microsoft.AspNetCore.Identity;
using StoreSystem.Application.Contract.Auth.Login.req;
using StoreSystem.Application.Contract.Auth.Res;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Interfaces.Auth;
using StoreSystem.Core.Entities;
using StoreSystem.Core.Interfaces;


namespace StoreSystem.Application.Services.AuthService.Login
{
    public class GoogleLoginService : ILogin<AuthRes,GoogleLoginReq>
    {
        private readonly IGoogleLoginExtension _googleLoginExtension;
        private readonly IToken _tokenService;
        private readonly IRoleExtension _roleService;
        private readonly IUserClaimsExtension _claimsService;
        private readonly IRepository<Store> _storeRepository;

        public GoogleLoginService(
            IGoogleLoginExtension googleLoginExtension,
            IRoleExtension roleService,
            IUserClaimsExtension claimsService,
            IToken tokenService,
            IRepository<Store> storeRepository)
        {
            _googleLoginExtension = googleLoginExtension;
            _roleService = roleService;
            _claimsService = claimsService;
            _tokenService = tokenService;
            _storeRepository = storeRepository;
        }
        
        public async Task<GeneralResponse<AuthRes>> Login(GoogleLoginReq model)
        {
            ApplicationUser? User = await _googleLoginExtension.LoginWithGoogleAsync(model.IdToken);
            string role = Roles.User;
            await _roleService.EnsureUserInRoleAsync(User!, role);

            await _claimsService.AddDefaultClaimsAsync(User!, role, 0); // StoreId will be set later when user creates/joins a store

            // Check if user has a store
            var userStore = await _storeRepository.FindAsync(s => s.UserId == User!.Id);
            bool isTemporary = userStore == null;

            string accessToken = _tokenService.GenerateAccessToken(await _claimsService.GetClaimsAsync(User!), isTemporary);
            string refreshToken = _tokenService.GenerateRefreshToken();

            var (isAdded, message) = await _tokenService.AddTokenToDB(User!, refreshToken);
            if (!isAdded)
                return GeneralResponse<AuthRes>.Failure($"Internal Server Error {message}", 501);

            return GeneralResponse<AuthRes>.Success(new AuthRes { Token = accessToken, RefreshToken = refreshToken },
                "Login with Google successful", 200);

        }
    }

}