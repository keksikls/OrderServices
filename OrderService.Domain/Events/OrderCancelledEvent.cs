using OrderService.Domain.Interfaces;

namespace OrderService.Domain.Events;

public record OrderCancelledEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public DateTime OccurredOn { get; }
    public DateTime CancelledDate  { get; }

    public OrderCancelledEvent(Guid orderId, DateTime occurredOn, DateTime cancelledDate)
    {
        OrderId = orderId;
        OccurredOn = occurredOn;
        CancelledDate = cancelledDate;
    }
}