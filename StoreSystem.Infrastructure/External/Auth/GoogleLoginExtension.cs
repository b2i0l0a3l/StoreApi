using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using StoreSystem.Core.Entities;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Infrastructure.External.Auth
{
    public class GoogleLoginExtension: IGoogleLoginExtension
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public GoogleLoginExtension(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ApplicationUser?> LoginWithGoogleAsync(string IdToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(IdToken);
            ApplicationUser? user = await _userManager.FindByEmailAsync(payload.Email);
            
            if (user == null)
            {
                
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    GoogleId = payload.Subject,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                };
                await _userManager.CreateAsync(user);
            }
            return user;
        }
    }
}