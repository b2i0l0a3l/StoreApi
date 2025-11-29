using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StoreSystem.Core.Entities;

namespace StoreSystem.Application.Contract.Categories.req
{
    public record GetCategoryReq
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Name { get; set; }

    }

}