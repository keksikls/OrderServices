using MediatR;
using OrderService.Application.Models.Orders;

namespace OrderService.Application.Cqrs.Commands.UpdateOrder;

public record UpdateOrderCommand(OrderDto orderDto, CancellationToken ct) : IRequest;