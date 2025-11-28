using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.User.req;
using StoreSystem.Application.Contract.User.res;

namespace StoreSystem.Application.Interfaces
{
    public interface IUserService
    {
         Task<GeneralResponse<ProfileRes>> Profile(string UserID);

        Task<GeneralResponse<string?>> UpdateProfile(string UserId,UpdateUserProfileReq req);
        Task<GeneralResponse<string?>> ChangePassword(string UserId,ChangePasswordReq req);

    }
}