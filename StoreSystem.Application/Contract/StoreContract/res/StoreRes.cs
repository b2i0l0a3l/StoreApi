using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Application.Contract.StoreContract.res
{
    public record StoreRes
    {   
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public string UserId { get; set; }= string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; } 
        public string? NewToken { get; set; }
    }
}