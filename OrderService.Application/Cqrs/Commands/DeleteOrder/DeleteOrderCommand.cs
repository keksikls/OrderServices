using MediatR;

namespace OrderService.Application.Cqrs.Commands.DeleteOrder;

public record DeleteOrderCommand(Guid id, CancellationToken ct) : IRequest;