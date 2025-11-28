using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.PurchaseContract.Req
{
    public class PurchaseItemReq
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
