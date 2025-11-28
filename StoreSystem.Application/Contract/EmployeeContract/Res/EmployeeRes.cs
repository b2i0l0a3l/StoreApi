using System;

namespace StoreSystem.Application.Contract.EmployeeContract.Res
{
    public class EmployeeRes
    {
        public int Id { get; init; }
        public string UserId { get; init; } = string.Empty;
        public int StoreId { get; init; }
        public string Role { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }
}
