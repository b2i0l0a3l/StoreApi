using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace StoreSystem.Core.Events.ProductEvent
{
    public class ProductCreatedEvent : INotification
    {
        public int Id { get; set; }
        public DateTime UpdateAt { get; set; }
        public int StockQuantity { get; set; }
        public int StoreId { get; set; }
        public string UserId { get; set; }
        
        public ProductCreatedEvent(int id , DateTime updateAt, int stockQuantity, int storeId, string userId)
        {
            Id = id;
            UpdateAt = updateAt;
            StockQuantity = stockQuantity;
            StoreId = storeId;
            UserId = userId;
        }
    }
}