namespace Binance.Domain.Dtos;

public record SymbolWrapperDto
{
    /// <summary>
    /// Stock symbol
    /// </summary>
    public string Symbol { get; set; } = null!;
}