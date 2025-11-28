using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Application.Contract.Auth.Login.req
{
    public class GoogleLoginReq
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
        
    }
}