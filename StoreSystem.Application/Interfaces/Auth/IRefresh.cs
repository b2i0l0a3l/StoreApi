using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Auth.Req;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.Token.req;

namespace StoreSystem.Application.Interfaces.Auth
{
    public interface IRefresh
    {
        Task<GeneralResponse<TokenReq>> Refresh(TokenReq model);
        
    }
}