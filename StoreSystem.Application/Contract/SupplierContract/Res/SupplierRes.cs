namespace StoreSystem.Application.Contract.SupplierContract.Res
{
    /// <summary>
    /// Response DTO for supplier.
    /// </summary>
    public class SupplierRes
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
