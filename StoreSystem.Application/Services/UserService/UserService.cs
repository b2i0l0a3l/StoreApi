using StoreSystem.Application.Contract.Common;
using StoreSystem.Core.Entities;
using Microsoft.AspNetCore.Identity;
using StoreSystem.Application.Contract.User.req;
using StoreSystem.Application.Contract.User.res;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Interfaces;

namespace StoreSystem.Application.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _CurrentUser;
        public UserService(ICurrentUserService CurrentUser,UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _CurrentUser = CurrentUser;
        }

        public async Task<GeneralResponse<string?>> ChangePassword( ChangePasswordReq req)
        {
            if (string.IsNullOrEmpty(_CurrentUser.UserId)) return GeneralResponse<string?>.Failure("Unauthorized", 401);
            ApplicationUser? user = await _userManager.FindByIdAsync(_CurrentUser.UserId);
            if (user == null) return GeneralResponse<string?>.Failure("User Not Found", 404);
            var result = await _userManager.ChangePasswordAsync(user, req.OldPassword!, req.NewPassword!);
            if (!result.Succeeded) return GeneralResponse<string?>.Failure($"{result.Errors}", 404);
            return GeneralResponse<string?>.Success(null, "Password changed successfully", 200);
        }
        public async Task<GeneralResponse<ProfileRes>> Profile()
        {
            if (string.IsNullOrEmpty(_CurrentUser.UserId)) return GeneralResponse<ProfileRes>.Failure("Unauthorized", 401);
            ApplicationUser? user = await _userManager.FindByIdAsync(_CurrentUser.UserId!);
            if (user == null) return GeneralResponse<ProfileRes>.Failure("User Not Found", 404);
            ProfileRes res = new ()
            { 
                Username = user.UserName,
                Email =  user.Email,
                FullName = $"{user.FirstName} {user.LastName}"
            };
            return GeneralResponse<ProfileRes>.Success(res," Profile fetched successfully", 200);
        }

        public async Task<GeneralResponse<string?>> UpdateProfile(UpdateUserProfileReq req)
        {
            if (string.IsNullOrEmpty(_CurrentUser.UserId)) return GeneralResponse<string?>.Failure("Unauthorized",401);
            ApplicationUser? user = await _userManager.FindByIdAsync(_CurrentUser.UserId);
            if (user == null) return GeneralResponse<string?>.Failure("User not found",404);

            user.FirstName = req.FirstName ?? user.FirstName;
            user.LastName = req.LastName ?? user.LastName;
            if (!string.IsNullOrEmpty(req.PhoneNumber)) user.PhoneNumber = req.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return GeneralResponse<string?>.Failure($"{result.Errors}", 404);
            return GeneralResponse<string?>.Success(null,"Profile updated successfully",200);
        }
    }

   
}