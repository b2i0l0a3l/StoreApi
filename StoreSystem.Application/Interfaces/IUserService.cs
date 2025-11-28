using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.User.req;
using StoreSystem.Application.Contract.User.res;

namespace StoreSystem.Application.Interfaces
{
    public interface IUserService
    {
         Task<GeneralResponse<ProfileRes>> Profile();

        Task<GeneralResponse<string?>> UpdateProfile(UpdateUserProfileReq req);
        Task<GeneralResponse<string?>> ChangePassword(ChangePasswordReq req);

    }
}