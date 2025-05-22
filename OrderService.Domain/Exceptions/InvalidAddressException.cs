namespace OrderService.Domain.Exceptions;

public class InvalidAddressException : Exception
{
    public InvalidAddressException(string fieldName)
        : base($"Invalid address: {fieldName}")
    {
    }

    public InvalidAddressException(string fieldName, string details)
        : base($"Invalid address: {fieldName}: {details}")
    {
    }
}