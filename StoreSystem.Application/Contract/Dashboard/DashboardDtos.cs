using System;
using System.Collections.Generic;

namespace StoreSystem.Application.Contract.Dashboard
{
    public record MetricDto<T>(T Value);

    public class TopProductDto
    {
        public int ProductId { get; init; }
        public string? ProductName { get; init; }
        public decimal TotalRevenue { get; init; }
        public decimal TotalQuantity { get; init; }
    }

    public class TopCustomerDto
    {
        public int CustomerId { get; init; }
        public string? CustomerName { get; init; }
        public decimal TotalSpent { get; init; }
    }

    public class LowStockProductDto
    {
        public int ProductId { get; init; }
        public string? ProductName { get; init; }
        public decimal Quantity { get; init; }
    }

    public class StockMovementDto
    {
        public int Id { get; init; }
        public int ProductId { get; init; }
        public string? ProductName { get; init; }
        public string? MovementType { get; init; }
        public int Quantity { get; init; }
        public DateTime OccurredAt { get; init; }
        public string? Reference { get; init; }
    }

    public class TransactionDto
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public string? Reference { get; init; }
        public decimal Total { get; init; }
        public int? CustomerId { get; init; }
        public string? CustomerName { get; init; }
    }

    public class PaymentDto
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public decimal Amount { get; init; }
        public string? Type { get; init; }
        public int? RelatedEntityId { get; init; }
        public string? Notes { get; init; }
    }

    public class DebtDto
    {
        public int EntityId { get; init; }
        public string? EntityName { get; init; }
        public decimal Outstanding { get; init; }
    }

    public class ProfitDto
    {
        public decimal Revenue { get; init; }
        public decimal Cost { get; init; }
        public decimal Profit => Revenue - Cost;
    }
}
