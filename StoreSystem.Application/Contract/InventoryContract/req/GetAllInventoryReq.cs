using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Application.Contract.InventoryContract.req
{
    public class GetAllInventoryReq
    {
        public int StoreId{ get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

    }
}