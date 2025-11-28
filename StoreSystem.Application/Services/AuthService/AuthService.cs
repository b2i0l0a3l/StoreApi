using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Auth.Req;
using StoreSystem.Application.Contract.Auth.Res;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.Token.req;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Interfaces.Auth;
using StoreSystem.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StoreSystem.Application.Contract.Auth.Login.req;

namespace StoreSystem.Application.Services.authService
{
    public class AuthService : IAuth
    {
        private readonly ILogin<AuthRes,LoginModel> _LoginService;
        private readonly ILogin<AuthRes,GoogleLoginReq> _LoginWithGoogleService;
        private readonly IRegister _register;
        private readonly IRefresh _refresh;
        
        public AuthService(ILogin<AuthRes,GoogleLoginReq> loginWithGoogle,ILogin<AuthRes,LoginModel> loginService, IRegister register, IRefresh refresh)
        {
            _LoginWithGoogleService = loginWithGoogle;
            _LoginService = loginService;
            _register = register;
            _refresh = refresh;
        }


        public async Task<GeneralResponse<AuthRes>> Register(SingUp model)
            => await _register.Register(model);

        public async Task<GeneralResponse<TokenReq>> Refresh(TokenReq model)
        => await _refresh.Refresh(model);

        public async Task<GeneralResponse<AuthRes>> Login<TRequest>(TRequest model)
        {
            if (model is LoginModel login)
            {
                return await _LoginService.Login(login);
            }
            else if (model is GoogleLoginReq google)
            {
                return await _LoginWithGoogleService.Login(google);
            }
            return GeneralResponse<AuthRes>.Failure("Invalid login model", 400);
        }

    }
}