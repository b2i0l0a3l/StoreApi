using StoreSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Application.Contract.Dashboard;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using BookingSystem.Core.enums;

namespace StoreSystem.Application.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<Inventory> _inventoryRepo;
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<SalesInvoice> _salesRepo;
        private readonly IRepository<PurchaseInvoice> _purchaseRepo;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<Supplier> _supplierRepo;
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<StockMovement> _movementRepo;
        private readonly ICurrentUserService _currentUserService;

        public DashboardService(
            IRepository<Product> productRepo,
            IRepository<Inventory> inventoryRepo,
            IRepository<Stock> stockRepo,
            IRepository<SalesInvoice> salesRepo,
            IRepository<PurchaseInvoice> purchaseRepo,
            IRepository<Customer> customerRepo,
            IRepository<Supplier> supplierRepo,
            IRepository<Payment> paymentRepo,
            IRepository<StockMovement> movementRepo,
            ICurrentUserService currentUserService)
        {
            _productRepo = productRepo;
            _inventoryRepo = inventoryRepo;
            _stockRepo = stockRepo;
            _salesRepo = salesRepo;
            _purchaseRepo = purchaseRepo;
            _customerRepo = customerRepo;
            _supplierRepo = supplierRepo;
            _paymentRepo = paymentRepo;
            _movementRepo = movementRepo;
            _currentUserService = currentUserService;
        }

        private static (DateTime from, DateTime to) ResolvePeriod(string period, DateTime? start, DateTime? end)
        {
            if (start.HasValue && end.HasValue) return (start.Value.Date, end.Value.Date.AddDays(1).AddTicks(-1));
            var now = DateTime.UtcNow;
            return period?.ToLower() switch
            {
                "year" => (new DateTime(now.Year, 1, 1), new DateTime(now.Year, 12, 31, 23, 59, 59)),
                _ => (new DateTime(now.Year, now.Month, 1), now)
            };
        }

        public async Task<GeneralResponse<int>> GetTotalProductsAsync()
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<int>.Failure("Unauthorized", 401);
            var count = await _productRepo.Query().Where(p => p.StoreId == _currentUserService.StoreId.Value).CountAsync();
            return GeneralResponse<int>.Success(count);
        }

        public async Task<GeneralResponse<int>> GetTotalInventoriesAsync()
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<int>.Failure("Unauthorized", 401);
            var count = await _inventoryRepo.Query().Where(i => i.StoreId == _currentUserService.StoreId.Value).CountAsync();
            return GeneralResponse<int>.Success(count);
        }

        public async Task<GeneralResponse<decimal>> GetTotalStockQuantityAsync()
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<decimal>.Failure("Unauthorized", 401);
            var total = await _stockRepo.Query().Where(s => s.Inventory!.StoreId == _currentUserService.StoreId.Value).Select(s => (decimal?)s.Quantity).SumAsync() ?? 0m;
            return GeneralResponse<decimal>.Success(total);
        }

        public async Task<GeneralResponse<decimal>> GetTotalSalesAsync(string period = "month", DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<decimal>.Failure("Unauthorized", 401);
            var (from, to) = ResolvePeriod(period, startDate, endDate);
            var total = await _salesRepo.Query().Where(s => s.StoreId == _currentUserService.StoreId.Value && s.Date >= from && s.Date <= to).Select(s => (decimal?)s.TotalAmount).SumAsync() ?? 0m;
            return GeneralResponse<decimal>.Success(total);
        }

        public async Task<GeneralResponse<decimal>> GetTotalPurchasesAsync(string period = "month", DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<decimal>.Failure("Unauthorized", 401);
            var (from, to) = ResolvePeriod(period, startDate, endDate);
            var total = await _purchaseRepo.Query().Where(p => p.StoreId == _currentUserService.StoreId.Value && p.Date >= from && p.Date <= to).Select(p => (decimal?)p.TotalAmount).SumAsync() ?? 0m;
            return GeneralResponse<decimal>.Success(total);
        }

        public async Task<GeneralResponse<int>> GetTotalCustomersAsync()
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<int>.Failure("Unauthorized", 401);
            var count = await _customerRepo.Query().Where(c => c.StoreId == _currentUserService.StoreId.Value).CountAsync();
            return GeneralResponse<int>.Success(count);
        }

        public async Task<GeneralResponse<int>> GetTotalSuppliersAsync()
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<int>.Failure("Unauthorized", 401);
            var count = await _supplierRepo.Query().Where(s => s.StoreId == _currentUserService.StoreId.Value).CountAsync();
            return GeneralResponse<int>.Success(count);
        }

        public async Task<GeneralResponse<decimal>> GetTotalPaymentsAsync(string type = "sale", DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<decimal>.Failure("Unauthorized", 401);
            var q = _paymentRepo.Query().Where(p => p.StoreId == _currentUserService.StoreId.Value);
            if (!string.IsNullOrWhiteSpace(type))
            {
                var t = type.ToLower() switch
                {
                    "purchase" => PaymentType.Purchase,
                    _ => PaymentType.Sale
                };
                q = q.Where(p => p.Type == t);
            }
            if (startDate.HasValue && endDate.HasValue)
            {
                var from = startDate.Value.Date;
                var to = endDate.Value.Date.AddDays(1).AddTicks(-1);
                q = q.Where(p => p.Date >= from && p.Date <= to);
            }
            var total = await q.Select(p => (decimal?)p.Amount).SumAsync() ?? 0m;
            return GeneralResponse<decimal>.Success(total);
        }

        public async Task<GeneralResponse<int>> GetLowStockAlertsCountAsync()
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<int>.Failure("Unauthorized", 401);
            var count = await _stockRepo.Query().Where(s => s.Inventory!.StoreId == _currentUserService.StoreId.Value).CountAsync(s => s.Quantity <= 0);
            return GeneralResponse<int>.Success(count);
        }

        public async Task<GeneralResponse<IEnumerable<DebtDto>>> GetCustomerDebtsAsync()
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<IEnumerable<DebtDto>>.Failure("Unauthorized", 401);
            var invoices = await _salesRepo.Query().Where(s => s.StoreId == _currentUserService.StoreId.Value).GroupBy(s => s.CustomerId).Select(g => new { CustomerId = g.Key, Total = g.Sum(x => x.TotalAmount) }).ToListAsync();
            var payments = await _paymentRepo.Query().Where(p => p.CustomerId != null && p.StoreId == _currentUserService.StoreId.Value).GroupBy(p => p.CustomerId).Select(g => new { CustomerId = g.Key, Paid = g.Sum(x => x.Amount) }).ToListAsync();
            var mapPayments = payments.ToDictionary(x => x.CustomerId!.Value, x => x.Paid);
            var result = new List<DebtDto>();
            foreach (var inv in invoices)
            {
                var paid = mapPayments.TryGetValue(inv.CustomerId, out var p) ? p : 0m;
                var outstanding = inv.Total - paid;
                if (outstanding <= 0) continue;
                var customer = await _customerRepo.Query().Where(c => c.Id == inv.CustomerId && c.StoreId == _currentUserService.StoreId.Value).Select(c => new { c.Id, c.Name }).FirstOrDefaultAsync();
                result.Add(new DebtDto { EntityId = customer?.Id ?? inv.CustomerId, EntityName = customer?.Name, Outstanding = outstanding });
            }
            return GeneralResponse<IEnumerable<DebtDto>>.Success(result);
        }

        public async Task<GeneralResponse<IEnumerable<DebtDto>>> GetSupplierDebtsAsync()
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<IEnumerable<DebtDto>>.Failure("Unauthorized", 401);
            var invoices = await _purchaseRepo.Query().Where(p => p.StoreId == _currentUserService.StoreId.Value).GroupBy(p => p.SupplierId).Select(g => new { SupplierId = g.Key, Total = g.Sum(x => x.TotalAmount) }).ToListAsync();
            var payments = await _paymentRepo.Query().Where(p => p.SupplierId != null && p.StoreId == _currentUserService.StoreId.Value).GroupBy(p => p.SupplierId).Select(g => new { SupplierId = g.Key, Paid = g.Sum(x => x.Amount) }).ToListAsync();
            var mapPayments = payments.ToDictionary(x => x.SupplierId!.Value, x => x.Paid);
            var result = new List<DebtDto>();
            foreach (var inv in invoices)
            {
                var paid = mapPayments.TryGetValue(inv.SupplierId, out var p) ? p : 0m;
                var outstanding = inv.Total - paid;
                if (outstanding <= 0) continue;
                var supplier = await _supplierRepo.Query().Where(c => c.Id == inv.SupplierId && c.StoreId == _currentUserService.StoreId.Value).Select(c => new { c.Id, c.Name }).FirstOrDefaultAsync();
                result.Add(new DebtDto { EntityId = supplier?.Id ?? inv.SupplierId, EntityName = supplier?.Name, Outstanding = outstanding });
            }
            return GeneralResponse<IEnumerable<DebtDto>>.Success(result);
        }

        public async Task<GeneralResponse<ProfitDto>> GetProfitAsync(string period = "month", DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<ProfitDto>.Failure("Unauthorized", 401);
            var (from, to) = ResolvePeriod(period, startDate, endDate);
            var revenue = await _salesRepo.Query().Where(s => s.StoreId == _currentUserService.StoreId.Value && s.Date >= from && s.Date <= to).Select(s => (decimal?)s.TotalAmount).SumAsync() ?? 0m;
            var cost = await _purchaseRepo.Query().Where(p => p.StoreId == _currentUserService.StoreId.Value && p.Date >= from && p.Date <= to).Select(p => (decimal?)p.TotalAmount).SumAsync() ?? 0m;
            var dto = new ProfitDto { Revenue = revenue, Cost = cost };
            return GeneralResponse<ProfitDto>.Success(dto);
        }

        public async Task<GeneralResponse<IEnumerable<TopProductDto>>> GetTopProductsAsync(int limit = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<IEnumerable<TopProductDto>>.Failure("Unauthorized", 401);
            var q = _salesRepo.Query().Where(s => s.StoreId == _currentUserService.StoreId.Value).SelectMany(s => s.SalesItems);
            if (startDate.HasValue && endDate.HasValue)
            {
                var from = startDate.Value.Date;
                var to = endDate.Value.Date.AddDays(1).AddTicks(-1);
                q = q.Where(i => i.SalesInvoice.Date >= from && i.SalesInvoice.Date <= to);
            }
            var groups = await q.GroupBy(i => new { i.ProductId, i.Product.Name }).Select(g => new TopProductDto { ProductId = g.Key.ProductId, ProductName = g.Key.Name, TotalQuantity = g.Sum(x => x.Qty), TotalRevenue = g.Sum(x => x.Total) }).OrderByDescending(x => x.TotalQuantity).Take(limit).ToListAsync();
            return GeneralResponse<IEnumerable<TopProductDto>>.Success(groups);
        }

        public async Task<GeneralResponse<IEnumerable<TopCustomerDto>>> GetTopCustomersAsync(int limit = 10, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<IEnumerable<TopCustomerDto>>.Failure("Unauthorized", 401);
            var q = _salesRepo.Query().Where(s => s.StoreId == _currentUserService.StoreId.Value);
            if (startDate.HasValue && endDate.HasValue)
            {
                var from = startDate.Value.Date;
                var to = endDate.Value.Date.AddDays(1).AddTicks(-1);
                q = q.Where(s => s.Date >= from && s.Date <= to);
            }
            var groups = await q.GroupBy(s => new { s.CustomerId, s.Customer.Name }).Select(g => new TopCustomerDto { CustomerId = g.Key.CustomerId, CustomerName = g.Key.Name, TotalSpent = g.Sum(x => x.TotalAmount) }).OrderByDescending(x => x.TotalSpent).Take(limit).ToListAsync();
            return GeneralResponse<IEnumerable<TopCustomerDto>>.Success(groups);
        }

        public async Task<GeneralResponse<IEnumerable<LowStockProductDto>>> GetLowStockProductsAsync(int page = 1, int pageSize = 50)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<IEnumerable<LowStockProductDto>>.Failure("Unauthorized", 401);
            var q = _stockRepo.Query().Where(s => s.Inventory!.StoreId == _currentUserService.StoreId.Value && s.Quantity <= 0).OrderBy(s => s.Quantity).Skip((page - 1) * pageSize).Take(pageSize).Select(s => new LowStockProductDto { ProductId = s.ProductId, ProductName = s.Product != null ? s.Product.Name : null, Quantity = s.Quantity });
            var list = await q.ToListAsync();
            return GeneralResponse<IEnumerable<LowStockProductDto>>.Success(list);
        }

        public async Task<GeneralResponse<IEnumerable<DebtDto>>> GetSupplierDebtsPagedAsync(int page = 1, int pageSize = 50)
        {
            var debts = (await GetSupplierDebtsAsync()).Data ?? Enumerable.Empty<DebtDto>();
            var pageList = debts.Skip((page - 1) * pageSize).Take(pageSize);
            return GeneralResponse<IEnumerable<DebtDto>>.Success(pageList);
        }

        public async Task<GeneralResponse<IEnumerable<StockMovementDto>>> GetRecentStockMovementsAsync(int limit = 20)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<IEnumerable<StockMovementDto>>.Failure("Unauthorized", 401);
            var q = _movementRepo.Query().Where(m => m.Product!.StoreId == _currentUserService.StoreId.Value).OrderByDescending(m => m.Date).Take(limit).Select(m => new StockMovementDto { Id = m.Id, ProductId = m.ProductId, ProductName = m.Product != null ? m.Product.Name : null, MovementType = m.Type.ToString(), Quantity = m.Qty, OccurredAt = m.Date, Reference = m.ReferenceId != null ? m.ReferenceId.ToString() : null });
            var list = await q.ToListAsync();
            return GeneralResponse<IEnumerable<StockMovementDto>>.Success(list);
        }

        public async Task<GeneralResponse<IEnumerable<TransactionDto>>> GetRecentSalesAsync(int limit = 20)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<IEnumerable<TransactionDto>>.Failure("Unauthorized", 401);
            var q = _salesRepo.Query().Where(s => s.StoreId == _currentUserService.StoreId.Value).OrderByDescending(s => s.Date).Take(limit).Select(s => new TransactionDto { Id = s.Id, Date = s.Date, Reference = null, Total = s.TotalAmount, CustomerId = s.CustomerId, CustomerName = s.Customer != null ? s.Customer.Name : null });
            var list = await q.ToListAsync();
            return GeneralResponse<IEnumerable<TransactionDto>>.Success(list);
        }

        public async Task<GeneralResponse<IEnumerable<TransactionDto>>> GetRecentPurchasesAsync(int limit = 20)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<IEnumerable<TransactionDto>>.Failure("Unauthorized", 401);
            var q = _purchaseRepo.Query().Where(p => p.StoreId == _currentUserService.StoreId.Value).OrderByDescending(s => s.Date).Take(limit).Select(p => new TransactionDto { Id = p.Id, Date = p.Date, Reference = null, Total = p.TotalAmount, CustomerId = p.SupplierId, CustomerName = p.Supplier != null ? p.Supplier.Name : null });
            var list = await q.ToListAsync();
            return GeneralResponse<IEnumerable<TransactionDto>>.Success(list);
        }

        public async Task<GeneralResponse<IEnumerable<PaymentDto>>> GetRecentPaymentsAsync(int limit = 20)
        {
            if (!_currentUserService.StoreId.HasValue) return GeneralResponse<IEnumerable<PaymentDto>>.Failure("Unauthorized", 401);
            var q = _paymentRepo.Query().Where(p => p.StoreId == _currentUserService.StoreId.Value).OrderByDescending(p => p.Date).Take(limit).Select(p => new PaymentDto { Id = p.Id, Date = p.Date, Amount = p.Amount, Type = p.Type.ToString(), RelatedEntityId = p.CustomerId ?? p.SupplierId, Notes = p.Note });
            var list = await q.ToListAsync();
            return GeneralResponse<IEnumerable<PaymentDto>>.Success(list);
        }
    }
}
