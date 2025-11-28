using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Dashboard;
using StoreSystem.Application.Contract.Common;

namespace StoreSystem.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<GeneralResponse<int>> GetTotalProductsAsync();
        Task<GeneralResponse<int>> GetTotalInventoriesAsync();
        Task<GeneralResponse<decimal>> GetTotalStockQuantityAsync();
        Task<GeneralResponse<decimal>> GetTotalSalesAsync(string period = "month", DateTime? startDate = null, DateTime? endDate = null);
        Task<GeneralResponse<decimal>> GetTotalPurchasesAsync(string period = "month", DateTime? startDate = null, DateTime? endDate = null);
        Task<GeneralResponse<int>> GetTotalCustomersAsync();
        Task<GeneralResponse<int>> GetTotalSuppliersAsync();
        Task<GeneralResponse<decimal>> GetTotalPaymentsAsync(string type = "received", DateTime? startDate = null, DateTime? endDate = null);
        Task<GeneralResponse<int>> GetLowStockAlertsCountAsync();
        Task<GeneralResponse<IEnumerable<DebtDto>>> GetCustomerDebtsAsync();
        Task<GeneralResponse<IEnumerable<DebtDto>>> GetSupplierDebtsAsync();
        Task<GeneralResponse<ProfitDto>> GetProfitAsync(string period = "month", DateTime? startDate = null, DateTime? endDate = null);

        Task<GeneralResponse<IEnumerable<TopProductDto>>> GetTopProductsAsync(int limit = 10, DateTime? startDate = null, DateTime? endDate = null);
        Task<GeneralResponse<IEnumerable<TopCustomerDto>>> GetTopCustomersAsync(int limit = 10, DateTime? startDate = null, DateTime? endDate = null);
        Task<GeneralResponse<IEnumerable<LowStockProductDto>>> GetLowStockProductsAsync(int page = 1, int pageSize = 50);
        Task<GeneralResponse<IEnumerable<DebtDto>>> GetSupplierDebtsPagedAsync(int page = 1, int pageSize = 50);

        Task<GeneralResponse<IEnumerable<StockMovementDto>>> GetRecentStockMovementsAsync(int limit = 20);
        Task<GeneralResponse<IEnumerable<TransactionDto>>> GetRecentSalesAsync(int limit = 20);
        Task<GeneralResponse<IEnumerable<TransactionDto>>> GetRecentPurchasesAsync(int limit = 20);
        Task<GeneralResponse<IEnumerable<PaymentDto>>> GetRecentPaymentsAsync(int limit = 20);
    }
}
