using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.SaleContract.Req
{
    public class SaleItemReq
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
