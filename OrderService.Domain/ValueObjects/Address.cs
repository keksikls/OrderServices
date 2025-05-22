namespace OrderService.Domain.ValueObjects;

public sealed record Address
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    private Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    public static Address Create(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new InvalidAddressException(nameof(street));
        }
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new InvalidAddressException(nameof(city));
        }
        if (string.IsNullOrWhiteSpace(state))
        {
            throw new InvalidAddressException(nameof(state));
        }
        if (string.IsNullOrWhiteSpace(postalCode))
        {
            throw new InvalidAddressException(nameof(postalCode));
        }
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new InvalidAddressException(nameof(country));
        }

        return new Address(street, city, state, postalCode, country);
    }
    
    public override string ToString() => 
        $"{Street}, {City}, {State}, {PostalCode}, {Country}";
};