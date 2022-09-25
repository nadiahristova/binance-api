namespace Binance.Domain.Dtos
{
    public class SymbolPriceDto
    {
        public string Symbol { get; set; } = null!;

        public DateTime TradeTime { get; set; }

        public decimal Price { get; set; }
    }
}
