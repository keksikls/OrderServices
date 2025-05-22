using OrderService.Domain.Entities;

namespace OrderService.Domain.Events;

public record OrderCreatedEvent(
    Guid ProductId,
    Guid OrderId,
    int Quantity,
    DateTime OccurredOn
) : IDomainEvent;