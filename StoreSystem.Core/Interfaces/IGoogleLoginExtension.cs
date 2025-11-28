using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Core.Entities;

namespace StoreSystem.Core.Interfaces
{
    public interface IGoogleLoginExtension
    {
        Task<ApplicationUser?> LoginWithGoogleAsync(string IdToken);
    }
}