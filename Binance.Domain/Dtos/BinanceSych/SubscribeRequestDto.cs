using Binance.Domain.Utils;
using Newtonsoft.Json;

namespace Binance.Domain.Dtos.BinanceSych;

public record SubscribeRequestDto
{
    [JsonProperty("method")]
    public string Method { get; init; } = Constants.BinanceSync.SubscribeMethod;

    [JsonProperty("params")]
    public IEnumerable<string> Parameters { get; set; } = null!;

    [JsonProperty("id")]
    public int Id { get; set; }
}