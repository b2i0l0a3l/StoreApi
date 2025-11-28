using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreSystem.Core.Entities;

namespace StoreSystem.Infrastructure.Persistence.Config
{
    /// <summary>
    /// Database index configuration for performance optimization
    /// </summary>
    public static class IndexConfiguration
    {
        public static void ConfigureIndexes(this ModelBuilder modelBuilder)
        {
            // Product Indexes
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .HasDatabaseName("IX_Product_Name");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Product_CategoryId");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.StoreId)
                .HasDatabaseName("IX_Product_StoreId");

            // Category Indexes
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.InventoryId)
                .HasDatabaseName("IX_Category_InventoryId");

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .HasDatabaseName("IX_Category_Name");

            // Store Indexes
            modelBuilder.Entity<Store>()
                .HasIndex(s => s.UserId)
                .HasDatabaseName("IX_Store_UserId");

            modelBuilder.Entity<Store>()
                .HasIndex(s => s.Name)
                .HasDatabaseName("IX_Store_Name");

            // Inventory Indexes
            modelBuilder.Entity<Inventory>()
                .HasIndex(i => i.StoreId)
                .HasDatabaseName("IX_Inventory_StoreId");

            // Stock Indexes
            modelBuilder.Entity<Stock>()
                .HasIndex(s => new { s.InventoryId, s.ProductId })
                .HasDatabaseName("IX_Stock_Inventory_Product");

            // SalesInvoice Indexes
            modelBuilder.Entity<SalesInvoice>()
                .HasIndex(s => s.CreatedAt)
                .HasDatabaseName("IX_SalesInvoice_CreatedAt");

            modelBuilder.Entity<SalesInvoice>()
                .HasIndex(s => s.CustomerId)
                .HasDatabaseName("IX_SalesInvoice_CustomerId");

            // PurchaseInvoice Indexes
            modelBuilder.Entity<PurchaseInvoice>()
                .HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_PurchaseInvoice_CreatedAt");

            modelBuilder.Entity<PurchaseInvoice>()
                .HasIndex(p => p.SupplierId)
                .HasDatabaseName("IX_PurchaseInvoice_SupplierId");

            // Customer Indexes
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Name)
                .HasDatabaseName("IX_Customer_Name");

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Phone)
                .HasDatabaseName("IX_Customer_Phone");
            
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .HasDatabaseName("IX_Customer_Email");

            // Supplier Indexes
            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Name)
                .HasDatabaseName("IX_Supplier_Name");

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Phone)
                .HasDatabaseName("IX_Supplier_Phone");
            
            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Email)
                .HasDatabaseName("IX_Supplier_Email");

            // StockMovement Indexes
            modelBuilder.Entity<StockMovement>()
                .HasIndex(sm => sm.ProductId)
                .HasDatabaseName("IX_StockMovement_ProductId");

            modelBuilder.Entity<StockMovement>()
                .HasIndex(sm => sm.InventoryId)
                .HasDatabaseName("IX_StockMovement_InventoryId");

            modelBuilder.Entity<StockMovement>()
                .HasIndex(sm => sm.Date)
                .HasDatabaseName("IX_StockMovement_Date");

            // SalesItem Indexes
            modelBuilder.Entity<SalesItem>()
                .HasIndex(si => si.ProductId)
                .HasDatabaseName("IX_SalesItem_ProductId");

            // PurchaseItem Indexes
            modelBuilder.Entity<PurchaseItem>()
                .HasIndex(pi => pi.ProductId)
                .HasDatabaseName("IX_PurchaseItem_ProductId");
        }
    }
}
