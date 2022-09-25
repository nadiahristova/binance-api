using Binance.Domain.Converters;
using Newtonsoft.Json;

namespace Binance.Domain.Dtos.BinanceSych;

public record TradeDataDto
{
    [JsonProperty("s")]
    public string Symbol { get; set; } = null!;

    [JsonProperty("T")]
    [JsonConverter(typeof(UTCDateTimeConverter))]
    public DateTime TradeTime { get; set; }

    [JsonProperty("p")]
    public decimal Price { get; set; }
}
