using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Auth.Res;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Interfaces.Auth;
using StoreSystem.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using BookingSystem.Core.common;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Application.Services.AuthService.Login
{
    public class LoginService : ILogin<AuthRes, LoginModel>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsExtension _userClaimsExtension;
        private readonly IToken _tokenService;
        private readonly IRepository<Store> _storeRepository;

        public LoginService(IUserClaimsExtension userClaims, UserManager<ApplicationUser> userManager, IToken tokenService, IRepository<Store> storeRepository)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _userClaimsExtension = userClaims;
            _storeRepository = storeRepository;
        }

        public async Task<GeneralResponse<AuthRes>> Login(LoginModel model)
        {
            if (model == null)
                return GeneralResponse<AuthRes>.Failure("Invalid login data", 400);
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);

                if (!await CheckEmailAndPassword(user, model.Password))
                {
                    return GeneralResponse<AuthRes>.Failure("Invalid email or password", 401);
                }

                var (token, refreshToken) = await GenerateJwtToken(user!);

                AuthRes authRes = new()
                {
                    Token = token,
                    RefreshToken = refreshToken
                };
                return GeneralResponse<AuthRes>.Success(authRes, "Login successful", 200);

            }
            catch (System.Exception ex)
            {

                return GeneralResponse<AuthRes>.Failure(ex.Message, 500);
            }
        }



        private async Task<bool> CheckEmailAndPassword(ApplicationUser? user, string Password)
        {
            if (user == null)
                return false;

            if (!await _userManager.CheckPasswordAsync(user, Password))
                return false;

            return true;
        }
        private async Task<(bool,string)> AddTokenToDB(ApplicationUser user, string refreshToken)
        {
            return await _tokenService.AddTokenToDB(user, refreshToken);
        }
      
     
        private async Task<List<Claim>> AddRoleToClaims(ApplicationUser user)
        {
            var AuthClaims = await _userClaimsExtension.GetClaimsAsync(user);

            foreach (var role in await _userManager.GetRolesAsync(user))
                AuthClaims.Add(new Claim(ClaimTypes.Role, role));

            var isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            AuthClaims.Add(new Claim("email_confirmed", isConfirmed.ToString()));
            
            return AuthClaims;
        }
        private async Task<(string, string)> GenerateJwtToken(ApplicationUser user)
        {
            // Check if user has a store
            var userStore = await _storeRepository.FindAsync(s => s.UserId == user.Id);
            bool isTemporary = userStore == null;

            var claims = await AddRoleToClaims(user);
            var token = _tokenService.GenerateAccessToken(claims, isTemporary);

            string refreshToken = _tokenService.GenerateRefreshToken();

            (bool , string) r = await AddTokenToDB(user, refreshToken);
            return (token, refreshToken);
        }

   
    }
}