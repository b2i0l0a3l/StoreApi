using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSystem.Core.Constants
{
    /// <summary>
    /// Permission codes for role-based access control
    /// </summary>
    public static class PermissionCodes
    {
        // Product Permissions
        public const string ProductCreate = "Product.Create";
        public const string ProductRead = "Product.Read";
        public const string ProductUpdate = "Product.Update";
        public const string ProductDelete = "Product.Delete";

        // Category Permissions
        public const string CategoryCreate = "Category.Create";
        public const string CategoryRead = "Category.Read";
        public const string CategoryUpdate = "Category.Update";
        public const string CategoryDelete = "Category.Delete";

        // Sales Permissions
        public const string SalesCreate = "Sales.Create";
        public const string SalesRead = "Sales.Read";
        public const string SalesUpdate = "Sales.Update";
        public const string SalesDelete = "Sales.Delete";

        // Purchase Permissions
        public const string PurchaseCreate = "Purchase.Create";
        public const string PurchaseRead = "Purchase.Read";
        public const string PurchaseUpdate = "Purchase.Update";
        public const string PurchaseDelete = "Purchase.Delete";

        // Stock Permissions
        public const string StockCreate = "Stock.Create";
        public const string StockRead = "Stock.Read";
        public const string StockUpdate = "Stock.Update";
        public const string StockDelete = "Stock.Delete";

        // Customer Permissions
        public const string CustomerCreate = "Customer.Create";
        public const string CustomerRead = "Customer.Read";
        public const string CustomerUpdate = "Customer.Update";
        public const string CustomerDelete = "Customer.Delete";

        // Supplier Permissions
        public const string SupplierCreate = "Supplier.Create";
        public const string SupplierRead = "Supplier.Read";
        public const string SupplierUpdate = "Supplier.Update";
        public const string SupplierDelete = "Supplier.Delete";

        // Dashboard & Reports
        public const string DashboardView = "Dashboard.View";
        public const string ReportsView = "Reports.View";

        // Store Management
        public const string StoreCreate = "Store.Create";
        public const string StoreRead = "Store.Read";
        public const string StoreUpdate = "Store.Update";
        public const string StoreDelete = "Store.Delete";

        // Employee Management
        public const string EmployeeCreate = "Employee.Create";
        public const string EmployeeRead = "Employee.Read";
        public const string EmployeeUpdate = "Employee.Update";
        public const string EmployeeDelete = "Employee.Delete";

        // Payment Permissions
        public const string PaymentCreate = "Payment.Create";
        public const string PaymentRead = "Payment.Read";
        public const string PaymentUpdate = "Payment.Update";
        public const string PaymentDelete = "Payment.Delete";

        // Return Permissions
        public const string ReturnCreate = "Return.Create";
        public const string ReturnRead = "Return.Read";

        /// <summary>
        /// Get all permission codes
        /// </summary>
        public static IEnumerable<string> GetAllPermissions()
        {
            return new[]
            {
                ProductCreate, ProductRead, ProductUpdate, ProductDelete,
                CategoryCreate, CategoryRead, CategoryUpdate, CategoryDelete,
                SalesCreate, SalesRead, SalesUpdate, SalesDelete,
                PurchaseCreate, PurchaseRead, PurchaseUpdate, PurchaseDelete,
                StockCreate, StockRead, StockUpdate, StockDelete,
                CustomerCreate, CustomerRead, CustomerUpdate, CustomerDelete,
                SupplierCreate, SupplierRead, SupplierUpdate, SupplierDelete,
                DashboardView, ReportsView,
                StoreCreate, StoreRead, StoreUpdate, StoreDelete,
                EmployeeCreate, EmployeeRead, EmployeeUpdate, EmployeeDelete,
                PaymentCreate, PaymentRead, PaymentUpdate, PaymentDelete,
                ReturnCreate, ReturnRead
            };
        }
    }
}
