namespace OrderService.Domain.ValueObjects;

public sealed record OrderName
{
    public string? Value { get; }
    
    private OrderName(string? value) => Value = value;

    public static OrderName Create(string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        
        if (value.Length != OrderNameConstraints.Length)
            throw OrderErrors.OrderName.InvalidLength(value.Length);
                
        if (!OrderNameConstraints.ValidChars.IsMatch(value))
            throw OrderErrors.OrderName.InvalidCharacters(value);
        
        return new OrderName(value);
    }
    
    public override string? ToString() => Value;
};