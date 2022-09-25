namespace Binance.Domain.Settings;

public record WatchDogSettings 
{
    public string Username { get; init; } = null!;

    public string Password { get; init; } = null!;
}
