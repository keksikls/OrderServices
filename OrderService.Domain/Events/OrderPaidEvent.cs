namespace OrderService.Domain.Events;

public record OrderPaidEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public DateTime PaidDate { get; }
    public DateTime OccurredOn { get; }

    public OrderPaidEvent(Guid orderId, DateTime paidDate, DateTime occurredOn)
    {
        OrderId = orderId;
        PaidDate = paidDate;
        OccurredOn = occurredOn;
    }
}