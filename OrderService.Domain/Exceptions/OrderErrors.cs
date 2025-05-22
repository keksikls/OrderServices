namespace OrderService.Domain.Exceptions;

public static class OrderErrors
{
    public static class OrderName
    {
        public static ArgumentOutOfRangeException InvalidLength(int actualLength)
            => new(nameof(actualLength), 
                $"Order name must be {OrderNameConstraints.Length} characters long. Given: {actualLength}");

        public static ArgumentException InvalidCharacters(string invalidValue)
            => new(
                $"Order name must contain only uppercase letters and digits. Given: {invalidValue}");
    }    
}

public static class OrderNameConstraints
{
    public const int Length = 5;
    public static readonly Regex ValidChars = new("^[A-Z0-9]+$");
}