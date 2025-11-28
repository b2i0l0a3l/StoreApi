using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Auth.Res;
using StoreSystem.Application.Contract.Common;

namespace StoreSystem.Application.Interfaces.Auth
{
    public interface IAuth : IRegister , IRefresh
    {
        Task<GeneralResponse<AuthRes>> Login<TRequest>(TRequest model);
    }
}