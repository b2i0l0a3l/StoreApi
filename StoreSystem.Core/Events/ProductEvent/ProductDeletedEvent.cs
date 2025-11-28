using MediatR;

namespace StoreSystem.Core.Events.ProductEvent
{
    public class ProductDeletedEvent : INotification
    {
        public int ProductId { get; }

        public ProductDeletedEvent(int productId)
        {
            ProductId = productId;
        }
    }
}
