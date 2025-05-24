using MediatR;
using OrderService.Application.Models.Orders;
using OrderService.Domain.Enum;

namespace OrderService.Application.Cqrs.Queries.GetOrdersByStatus;

public record GetOrdersByStatusQuery(OrderStatus Status)
    :IRequest<List<OrderDto>>;