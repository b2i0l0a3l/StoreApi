using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Application.Contract.User.res
{
    public record ProfileRes
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; } 
        public string? PhoneNumber { get; set; }
    }
}