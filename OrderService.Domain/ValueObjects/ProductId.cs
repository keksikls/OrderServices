namespace OrderService.Domain.ValueObjects;

public sealed record ProductId
{
    public Guid Value { get; }

    private ProductId(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException.InvalidProductIdException(value);
        
        Value = value;
    }

    public static ProductId Create(Guid value) => new(value);
    
    public override string ToString() => Value.ToString();
}