namespace OrderService.Domain.Events;

public record OrderRemoveItemEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid ProductId { get; }
    public DateTime OccurredOn { get; }

    public OrderRemoveItemEvent(Guid orderId, Guid productId, DateTime occurredOn)
    {
        OrderId = orderId;
        ProductId = productId;
        OccurredOn = occurredOn;
    }
}