using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Contract.ProductContract.Req
{
    public class GetProductReq
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageSize { get; set; } = 10;
    }

}