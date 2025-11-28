using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Application.Contract.StoreContract.req
{
    public record StoreReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;


        public string? Address { get; set; }

        public string? Phone { get; set; } 
    }
}