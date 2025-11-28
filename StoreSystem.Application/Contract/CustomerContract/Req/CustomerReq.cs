using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.CustomerContract.Req
{
    /// <summary>
    /// Request DTO to create or update a customer.
    /// </summary>
    public class CustomerReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Email { get; set; }
    }
}
