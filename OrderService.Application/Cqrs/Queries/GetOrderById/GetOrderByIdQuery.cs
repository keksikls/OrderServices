namespace OrderService.Application.Cqrs.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;