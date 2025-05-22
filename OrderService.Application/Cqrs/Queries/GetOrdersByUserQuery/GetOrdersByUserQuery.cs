namespace OrderService.Application.Cqrs.Queries.GetOrdersByUserQuery;

public record GetOrdersByUserQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedList<OrderDto>>;