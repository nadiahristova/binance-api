using Binance.Domain.Dtos.BinanceSych;
using Binance.Domain.Enums;

namespace Binance.Domain.Interfaces;
public interface ISymbolPriceService
{
    Task BulkImportTradeData(IEnumerable<TradeDataDto> tradeData, CancellationToken cancellationToken);
    Task<decimal> Get24hAvgPrice(string symbol, CancellationToken cancellationToken);
    Task<decimal> GetSimpleMovingAverage(string symbol, int dataPoints, TimePeriod timePeriod, DateTime? startDate, CancellationToken cancellationToken);
}
