using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Application.Contract.Categories.req
{
    public record CategoryReq 
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
        
    }
}