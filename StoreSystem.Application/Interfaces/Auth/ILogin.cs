using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Auth.Res;
using StoreSystem.Application.Contract.Common;

namespace StoreSystem.Application.Interfaces.Auth
{
    public interface ILogin<T,Q>
    {
        Task<GeneralResponse<T>> Login(Q model);

        
    }
}