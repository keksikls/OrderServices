namespace OrderService.Domain.Interfaces;

public interface IDomainEvent
{
    DateTime OccurredOn => DateTime.UtcNow;
}