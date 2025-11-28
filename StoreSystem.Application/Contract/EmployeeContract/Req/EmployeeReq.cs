using System;

namespace StoreSystem.Application.Contract.EmployeeContract.Req
{
    public class EmployeeReq
    {
        public string UserId { get; set; } = string.Empty;
        public int StoreId { get; set; }
        public int Role { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
