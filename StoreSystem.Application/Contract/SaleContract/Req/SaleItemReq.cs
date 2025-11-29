using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.SaleContract.Req
{
    public class SaleItemReq
    {
        [Required]
        [Range(1,int.MaxValue,ErrorMessage ="Product Id Must Greath than 0")]

        public int ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Qunatity Must Greath than 0")]
        public int Quantity { get; set; }
        [Range(1,int.MaxValue,ErrorMessage ="Product Id Must Greath than 0")]
        public decimal UnitPrice { get; set; }
    }
}
