namespace OrderService.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } // protected - для инкапсуляции
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    private List<IDomainEvent> _domainEvents;
    public IReadOnlyCollection<IDomainEvent>? DomainEvents => _domainEvents?.AsReadOnly();
    
    protected BaseEntity()
    {
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));
        
        _domainEvents ??= new List<IDomainEvent>();
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}