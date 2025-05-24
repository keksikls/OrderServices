using OrderService.Domain.Interfaces;

namespace OrderService.Domain.Events;

public record OrderShippedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public DateTime OccurredOn { get; }
    public DateTime ShippedDate { get; }

    public OrderShippedEvent(Guid orderId, DateTime occurredOn, DateTime shippedDate)
    {
        OrderId = orderId;
        OccurredOn = occurredOn;
        ShippedDate = shippedDate;
    }
}