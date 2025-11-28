namespace StoreSystem.Application.Contract.SaleContract.Res
{
    public class SaleItemRes
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
