using MediatR;

namespace OrderService.Application.Cqrs.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId, string Reason) : IRequest;