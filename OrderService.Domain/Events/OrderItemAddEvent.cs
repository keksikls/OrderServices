using OrderService.Domain.Interfaces;

namespace OrderService.Domain.Events;

public record OrderItemAddEvent(
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    decimal Price,
    DateTime OccurredOn
) : IDomainEvent;