namespace StoreSystem.Application.Contract.CustomerContract.Res
{
    /// <summary>
    /// Response DTO for customer.
    /// </summary>
    public class CustomerRes
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
