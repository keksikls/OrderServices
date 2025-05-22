using MediatR;

namespace OrderService.Application.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId, string Reason) : IRequest;