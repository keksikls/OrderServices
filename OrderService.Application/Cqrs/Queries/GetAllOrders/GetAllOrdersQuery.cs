using MediatR;
using OrderService.Application.Models.Orders;
using OrderService.Domain.Enum;
using OrderService.Domain.Models;

namespace OrderService.Application.Cqrs.Queries.GetAllOrders;

public record GetAllOrdersQuery(
    int Page = 1,
    int PageSize = 20,
    OrderStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null) : IRequest<PagedList<OrderDto>>;