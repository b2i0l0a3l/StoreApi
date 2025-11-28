namespace StoreSystem.Application.Contract.PurchaseContract.Res
{
    public class PurchaseItemRes
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal => Quantity * UnitCost;
    }
}
