using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.SupplierContract.Req
{
    /// <summary>
    /// Request DTO to create or update a supplier.
    /// </summary>
    public class SupplierReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? ContactName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }
    }
}
