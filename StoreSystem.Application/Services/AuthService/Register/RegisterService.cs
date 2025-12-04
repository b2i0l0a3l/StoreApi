using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using BookingSystem.Core.common;
using StoreSystem.Core.Interfaces;
using StoreSystem.Application.Contract.Auth.Req;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Interfaces.Auth;
using StoreSystem.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using StoreSystem.Application.Contract.Auth.Res;
using System.Security.Claims;

namespace StoreSystem.Application.Services.AuthService.Register
{
    public class RegisterService : IRegister
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IRoleExtension _roleService;
        private readonly IUserClaimsExtension _claimsService;
        private readonly IToken _tokenService;

        private ILogger<RegisterService> _logger;   

        public RegisterService(
            IRoleExtension roleService,
            IUserClaimsExtension claimsService,
            ILogger<RegisterService> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IToken tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _logger = logger;
            _roleService = roleService;
            _claimsService = claimsService;
            _tokenService = tokenService;
        }

        public async Task<GeneralResponse<AuthRes>> Register(SingUp model)
        {
            if (model == null)
                return GeneralResponse<AuthRes>.Failure("Invalid user data", 400);

            try
            {
                if (await CheckUserIfExists(model.Email))
                    return GeneralResponse<AuthRes>.Failure("User already exists", 409);


                ApplicationUser user = new()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = Roles.User
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return GeneralResponse<AuthRes>.Failure($"{string.Join(", ", result.Errors.Select(e => e.Description))}", 400);
                }

                string? RoleResult = await _roleService.EnsureUserInRoleAsync(user, Roles.User);
                if (RoleResult != null)
                    return GeneralResponse<AuthRes>.Failure(RoleResult??"Error", 400);
                
                await ConfirmEmail(user);

                // Generate temporary token
                var claims = await _claimsService.GetClaimsAsync(user);
                // Add default claims if not present
                if (!claims.Any(c => c.Type == ClaimTypes.Role))
                    claims.Add(new Claim(ClaimTypes.Role, Roles.User));
                
                var token = _tokenService.GenerateAccessToken(claims, isTemporary: true);
                var refreshToken = _tokenService.GenerateRefreshToken();
                await _tokenService.AddTokenToDB(user, refreshToken);

                return GeneralResponse<AuthRes>.Success(new AuthRes 
                { 
                    Token = token, 
                    RefreshToken = refreshToken,
                    IsTemporary = true 
                }, "User registered successfully. Please check your email to confirm.", 201);
            }
            catch (Exception ex)
            {
                return GeneralResponse<AuthRes>.Failure(ex.Message, 500);
            }

        }

        private async Task ConfirmEmail(ApplicationUser user)
        {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
                    var encodedToken = Convert.ToBase64String(tokenBytes)
                                        .Replace("+", "-")
                                        .Replace("/", "_")
                                        .Replace("=", "");
                string confirmUrl = $"http://localhost:5107/api/Auth/ConfirmEmail?userId={user.Id}&token={encodedToken}";
                string body = $"Please confirm your email by clicking <a href=\"{confirmUrl}\">this link</a> ";
                try
                {
                    await _emailSender.SendEmailAsync(user.Email!, "Confirm your email", body);
                }
                catch
                {
                    _logger.LogError("Failed to send confirmation email to {Email}", user.Email);
                }
        }

        private async Task<bool> CheckUserIfExists(string Email)
        {
            var IsUserExist = await _userManager.FindByNameAsync(Email);
            return IsUserExist != null;
        }
    }
}