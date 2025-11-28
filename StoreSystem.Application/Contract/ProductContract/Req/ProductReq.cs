using System.ComponentModel.DataAnnotations;

namespace StoreSystem.Application.Contract.ProductContract.Req
{
    /// <summary>
    /// Request DTO to create or update a product.
    /// </summary>
    public class ProductReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? SKU { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        public string? Barcode { get; set; }
        [Required]
        public decimal CostPrice { get; set; }
        [Required]

        public decimal SellPrice { get; set; }
        [Required]

        public int CategoryId { get; set; }
        [Required]

        public int StoreId { get; set; }
    }
}
