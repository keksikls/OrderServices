using OrderService.Domain.Exceptions;

namespace OrderService.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency = "USD")
    {
        if (amount <= 0)
        {
            throw new DomainException.InvalidPriceException(amount);
        }
        Currency = currency;
        Amount = amount;
    }

    public static Money Create(decimal amount, string currency = "USD")
    {
        if (amount <= 0)
        {
            throw new ArgumentNullException(nameof(amount), "Amount must be greater than zero");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required", nameof(currency));
        }
        
        return new Money(amount, currency);
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot add money with different currencies.");
        }

        return new Money(Amount + other.Amount, Currency);
    }

    public override string ToString() => $"{Amount} {Currency}";
}