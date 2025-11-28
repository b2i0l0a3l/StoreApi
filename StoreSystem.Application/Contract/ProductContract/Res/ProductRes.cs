using System;

namespace StoreSystem.Application.Contract.ProductContract.Res
{
    /// <summary>
    /// Product response DTO returned by application services.
    /// </summary>
    public class ProductRes
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public string? Barcode { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int CategoryId { get; set; }
        public int StoreId { get; set; }
        public int StockQuantity { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}

