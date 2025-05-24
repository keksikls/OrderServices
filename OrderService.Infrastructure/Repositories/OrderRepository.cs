using Microsoft.EntityFrameworkCore;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Application.Models.Orders;
using OrderService.Domain.Entities;
using OrderService.Domain.Enum;
using OrderService.Domain.Exceptions;
using OrderService.Infrastructure.Data.DbContext;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    public readonly OrderDbContext _orderDbContext;
    public readonly DbSet<Order> _orders;
    public readonly ICartsService _cartsService;

    public OrderRepository(OrderDbContext orderDbContext, ICartsService cartsService)
    {
        _orderDbContext = orderDbContext;
        _orders = orderDbContext.Orders;
        _cartsService = cartsService;
    }

    public async Task<OrderDto> Create(CreateOrderDto order)
    {
        var orderId = Guid.NewGuid(); // Генерация ID внутри метода
    
        var orderByOrderNumber = await _orders.FirstOrDefaultAsync(x =>
            x.Id == orderId && x.MerchantId == order.MerchantId);

        if (orderByOrderNumber != null)
        {
            throw new DuplicateEntityException($"Order with orderNubmer id is exsist for merchant {order.MerchantId}");
        }

        if (order.Cart == null)
        {
            throw new ArgumentNullException();
        }

        var cart = await _cartsService.Create(order.Cart);

        var entity = new Order
        {
            MerchantId = order.MerchantId,
            OrderName = order.OrderName,
            CustomerId = order.CustomerId,
            CartId = cart.Id
        };

        var orderSaveResult = await _orders.AddAsync(entity);
        await _orderDbContext.SaveChangesAsync();

        var orderEnityResult = orderSaveResult.Entity;

        return orderEnityResult.ToDto();
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _orders.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Order?> GetByIdWithCartAsync(Guid id, CancellationToken ct)
    {
        return await _orders
            .Include(x => x.Cart)
            .ThenInclude(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(Order order, CancellationToken ct)
    {
        await _orders.AddAsync(order, ct);
    }

    public Task UpdateAsync(Order order, CancellationToken ct)
    {
        _orderDbContext.Entry(order).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var order = await GetByIdAsync(id, ct);
        if (order != null)
            _orders.Remove(order);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _orderDbContext.SaveChangesAsync(ct);
    }

    public async Task<List<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct)
    {
        return await _orders
            .Where(o => o.Status == status)
            .Include(o => o.Cart)
            .ThenInclude(c => c.CartItems)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<(List<Order> Orders, int TotalCount)> GetByUserIdAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = _orders
            .Where(o => o.CustomerId == userId)
            .Include(o => o.Cart)
            .ThenInclude(c => c.CartItems)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(ct);
    
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (orders, totalCount);
    }
    
    public async Task<(List<Order> Orders, int TotalCount)> GetAllAsync(
        int page,
        int pageSize,
        OrderStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken ct = default)
    {
        var query = _orders
            .Include(o => o.Cart)
            .ThenInclude(c => c.CartItems)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= toDate.Value);
        }

        query = query.OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(ct);
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (orders, totalCount);
    }
}