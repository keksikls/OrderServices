using Microsoft.EntityFrameworkCore;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Application.Models.Orders;
using OrderService.Domain.Entities;
using OrderService.Domain.Enum;
using OrderService.Domain.Exceptions;
using OrderService.Infrastructure.Data.DbContext;
using OrderService.Infrastructure.Services;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _orderDbContext;
    private readonly DbSet<Order> _orders;
    private readonly ICartsService _cartsService;

    public OrderRepository(
        OrderDbContext orderDbContext,
        ICartsService cartsService)
    {
        _orderDbContext = orderDbContext;
        _orders = orderDbContext.Orders;
        _cartsService = cartsService;
    }

    public async Task<OrderDto> Create(CreateOrderDto order, CancellationToken ct)
    {
        var orderId = Guid.NewGuid();
    
        var orderByOrderNumber = await _orders.FirstOrDefaultAsync(x =>
            x.Id == orderId && x.MerchantId == order.MerchantId, ct);

        if (orderByOrderNumber != null)
        {
            throw new DuplicateEntityException($"Order with orderNubmer id is exsist for merchant {order.MerchantId}");
        }

        if (order.Cart == null)
        {
            throw new ArgumentNullException(nameof(order.Cart));
        }

        var cart = await _cartsService.Create(order.Cart, ct);

        var entity = new Order
        {
            MerchantId = order.MerchantId,
            OrderName = order.OrderName,
            CustomerId = order.CustomerId,
            CartId = cart.Id
        };

        var orderSaveResult = await _orders.AddAsync(entity, ct);
        await _orderDbContext.SaveChangesAsync(ct);
        

        return orderSaveResult.Entity.ToDto();
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

    public async Task UpdateAsync(OrderDto orderDto, CancellationToken ct)
    {
        _orderDbContext.Update(orderDto);
        _orderDbContext.Entry(orderDto).State = EntityState.Modified;

         if (orderDto != null)
         {
             await _orderDbContext.SaveChangesAsync(ct);
         }
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var order = await GetByIdAsync(id, ct);
        if (order != null)
        {
            _orders.Remove(order);
            _orderDbContext.SaveChangesAsync(ct);
        }
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        // Получаем все сущности с событиями до сохранения
        var entitiesWithEvents = _orderDbContext.ChangeTracker
            .Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents?.Any() == true)
            .ToList();

        // Сначала сохраняем изменения
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