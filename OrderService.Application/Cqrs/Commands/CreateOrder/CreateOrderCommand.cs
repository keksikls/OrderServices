using MediatR;
using OrderService.Application.Models.Orders;

namespace OrderService.Application.Cqrs.Commands.CreateOrder;

public record CreateOrderCommand(CreateOrderDto OrderDto) : IRequest<OrderDto>;