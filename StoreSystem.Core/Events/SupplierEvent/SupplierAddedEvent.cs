using MediatR;

namespace StoreSystem.Core.Events.SupplierEvent
{
    /// <summary>
    /// Event published when a supplier is added.
    /// </summary>
    public class SupplierAddedEvent : INotification
    {
        public int SupplierId { get; set; }
        public string Name { get; set; } = string.Empty;

        public SupplierAddedEvent(int supplierId, string name)
        {
            SupplierId = supplierId;
            Name = name;
        }
    }
}
