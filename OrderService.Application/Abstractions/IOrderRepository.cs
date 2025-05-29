using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Models.Orders;
using OrderService.Domain.Entities;
using OrderService.Domain.Enum;

namespace OrderService.Application.Abstractions;

public interface IOrderRepository
{
    Task<OrderDto> Create(CreateOrderDto order,CancellationToken ct);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Order?> GetByIdWithCartAsync(Guid id, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    Task UpdateAsync([FromBody] OrderDto orderDto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<List<Order>?> GetByStatusAsync(OrderStatus status, CancellationToken ct);
    Task<(List<Order> Orders, int TotalCount)> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken ct);
    Task<(List<Order> Orders, int TotalCount)> GetAllAsync(int page, int pageSize, OrderStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);
}