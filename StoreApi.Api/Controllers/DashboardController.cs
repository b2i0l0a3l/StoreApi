using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreSystem.Application.Interfaces;

namespace BookingApi.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboard;

        public DashboardController(IDashboardService dashboard)
        {
            _dashboard = dashboard;
        }

        [HttpGet("total-products")]
        public async Task<IActionResult> TotalProducts() => Ok(await _dashboard.GetTotalProductsAsync());

        [HttpGet("total-inventories")]
        public async Task<IActionResult> TotalInventories() => Ok(await _dashboard.GetTotalInventoriesAsync());

        [HttpGet("total-stock")]
        public async Task<IActionResult> TotalStock() => Ok(await _dashboard.GetTotalStockQuantityAsync());

        [HttpGet("total-sales")]
        public async Task<IActionResult> TotalSales([FromQuery] string period = "month", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null) => Ok(await _dashboard.GetTotalSalesAsync(period, startDate, endDate));

        [HttpGet("total-purchases")]
        public async Task<IActionResult> TotalPurchases([FromQuery] string period = "month", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null) => Ok(await _dashboard.GetTotalPurchasesAsync(period, startDate, endDate));

        [HttpGet("total-customers")]
        public async Task<IActionResult> TotalCustomers() => Ok(await _dashboard.GetTotalCustomersAsync());

        [HttpGet("total-suppliers")]
        public async Task<IActionResult> TotalSuppliers() => Ok(await _dashboard.GetTotalSuppliersAsync());

        [HttpGet("total-payments")]
        public async Task<IActionResult> TotalPayments([FromQuery] string type = "received", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null) => Ok(await _dashboard.GetTotalPaymentsAsync(type, startDate, endDate));

        [HttpGet("low-stock")]
        public async Task<IActionResult> LowStockAlerts() => Ok(await _dashboard.GetLowStockAlertsCountAsync());

        [HttpGet("customer-debts")]
        public async Task<IActionResult> CustomerDebts() => Ok(await _dashboard.GetCustomerDebtsAsync());

        [HttpGet("supplier-debts")]
        public async Task<IActionResult> SupplierDebts() => Ok(await _dashboard.GetSupplierDebtsAsync());

        [HttpGet("profit")]
        public async Task<IActionResult> Profit([FromQuery] string period = "month", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null) => Ok(await _dashboard.GetProfitAsync(period, startDate, endDate));

        [HttpGet("top-products")]
        public async Task<IActionResult> TopProducts([FromQuery] int limit = 10, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null) => Ok(await _dashboard.GetTopProductsAsync(limit, startDate, endDate));

        [HttpGet("top-customers")]
        public async Task<IActionResult> TopCustomers([FromQuery] int limit = 10, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null) => Ok(await _dashboard.GetTopCustomersAsync(limit, startDate, endDate));

        [HttpGet("low-stock-products")]
        public async Task<IActionResult> LowStockProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 50) => Ok(await _dashboard.GetLowStockProductsAsync(page, pageSize));

        [HttpGet("supplier-debts-table")]
        public async Task<IActionResult> SupplierDebtsTable([FromQuery] int page = 1, [FromQuery] int pageSize = 50) => Ok(await _dashboard.GetSupplierDebtsPagedAsync(page, pageSize));

        [HttpGet("recent-stock-movements")]
        public async Task<IActionResult> RecentStockMovements([FromQuery] int limit = 20) => Ok(await _dashboard.GetRecentStockMovementsAsync(limit));

        [HttpGet("recent-sales")]
        public async Task<IActionResult> RecentSales([FromQuery] int limit = 20) => Ok(await _dashboard.GetRecentSalesAsync(limit));

        [HttpGet("recent-purchases")]
        public async Task<IActionResult> RecentPurchases([FromQuery] int limit = 20) => Ok(await _dashboard.GetRecentPurchasesAsync(limit));

        [HttpGet("recent-payments")]
        public async Task<IActionResult> RecentPayments([FromQuery] int limit = 20) => Ok(await _dashboard.GetRecentPaymentsAsync(limit));
    }
}
