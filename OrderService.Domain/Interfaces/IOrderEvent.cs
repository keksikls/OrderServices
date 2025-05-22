namespace OrderService.Domain.Interfaces;

public interface IOrderEvent : IDomainEvent
{
    Guid OrderId { get; }
}