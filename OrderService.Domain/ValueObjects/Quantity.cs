using OrderService.Domain.Exceptions;

namespace OrderService.Domain.ValueObjects;

public sealed record Quantity
{
    public int Value { get; }

    private Quantity(int value)
    {
        if (value <= 0)
        {
            throw new DomainException.InvalidQuantityException(value);
        }
        
        Value = value;
    }
    
    public static Quantity Create(int value) => new(value);
    
    public Quantity Increase(int amount) => Create(amount + Value);
    public Quantity Update(int newValue) => Create(newValue);
    
    public override string ToString() => Value.ToString();
}