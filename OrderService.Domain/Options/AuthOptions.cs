namespace OrderService.Domain.Options;

public class AuthOptions
{
    public string TokenPrivateKey { get; set; } = null!;
    public long ExpireIntervalMinutes { get; set; }
}