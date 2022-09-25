using Newtonsoft.Json;

namespace Binance.Domain.Dtos.BinanceSych;

public record StreamDataDto
{
    [JsonProperty("stream")]
    public string Stream { get; set; } = null!;

    [JsonProperty("data")]
    public TradeDataDto Data { get; set; }
}