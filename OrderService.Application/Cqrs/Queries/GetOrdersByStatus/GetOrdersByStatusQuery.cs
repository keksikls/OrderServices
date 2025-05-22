namespace OrderService.Application.Cqrs.Queries.GetOrdersByStatus;

public record GetOrdersByStatusQuery(OrderStatus Status)
    :IRequest<List<OrderDto>>;