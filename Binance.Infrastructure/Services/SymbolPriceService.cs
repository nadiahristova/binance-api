using Binance.Domain.Dtos.BinanceSych;
using Binance.Domain.Entities;
using Binance.Domain.Enums;
using Binance.Domain.Exceptions.Custom;
using Binance.Domain.Interfaces;
using Binance.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Logging;

namespace Binance.Infrastructure.Services;

public class SymbolPriceService : ISymbolPriceService
{
	private readonly ISymbolRepository _symbolRepository;
	private readonly ISymbolPriceRepository _symbolPriceRepository;
	private readonly ILogger<SymbolPriceService> _logger;
	private readonly Lazy<IEnumerable<string>> _supportedSymbols = new Lazy<IEnumerable<string>>(()
	=> Enum.GetValues(typeof(Symbols)).Cast<Symbols>().Select(s => s.ToString().ToUpper()));

	public SymbolPriceService(ISymbolRepository symbolRepository,
		ISymbolPriceRepository symbolPriceRepository,
		ILogger<SymbolPriceService> logger)
	{
		_symbolRepository = symbolRepository;
		_symbolPriceRepository = symbolPriceRepository;
		_logger = logger;
	}

	public async Task BulkImportTradeData(IEnumerable<TradeDataDto> tradeData, CancellationToken cancellationToken)
	{
		var mappedData = tradeData.GroupBy(x => x.Symbol).ToDictionary(x => x.Key);

		var allSymbols = await _symbolRepository.Index();

		IDictionary<string, int> symbolToIdMapper = await ReturnSymbolMapper(mappedData.Keys.ToList(), allSymbols, cancellationToken).ConfigureAwait(false);

		foreach (var kv in mappedData)
		{
			var symbolName = kv.Key.ToUpper();

			if (symbolToIdMapper.ContainsKey(symbolName))
			{
				var symbolId = symbolToIdMapper[symbolName];

				var newEntries = kv.Value.AsParallel().Select(x => new SymbolPrice
				{
					SymbolId = symbolId,
					TradeTime = x.TradeTime,
					Price = x.Price
				}).ToList();

				await _symbolPriceRepository.BulkInsert(newEntries, cancellationToken).ConfigureAwait(false);
			}
		}

		await _symbolPriceRepository.SaveChanges(cancellationToken).ConfigureAwait(false);
	}

	public async Task<decimal> Get24hAvgPrice(string symbol, CancellationToken cancellationToken)
	{
		var fromDate = DateTime.UtcNow.AddDays(-1);

        return await GetAveragePrice(symbol, fromDate, cancellationToken).ConfigureAwait(false);
    }

	public async Task<decimal> GetSimpleMovingAverage(string symbol, int dataPoints, TimePeriod timePeriod, DateTime? startDate, CancellationToken cancellationToken)
	{
		var fromDate = DateTime.UtcNow - ReurnTimePeriod(timePeriod);

		if(startDate.HasValue) 
			fromDate = startDate < fromDate ? fromDate : startDate.Value;

        return await GetAveragePrice(symbol, fromDate, cancellationToken).ConfigureAwait(false);
    }

	private async Task<decimal> GetAveragePrice(string symbol, DateTime startDate, CancellationToken cancellationToken)
	{
        var allPrices = await _symbolRepository.GetAllPricesBySymbol(symbol, startDate, cancellationToken).ConfigureAwait(false);

		if (!allPrices.Any())
		{
			throw new NotFoundDomainException($"No prices found for symbol: '{symbol}' and period starting from date: {startDate}.");
		}

        return allPrices.Average();
    }

	private TimeSpan ReurnTimePeriod(TimePeriod timePeriod)
	=> timePeriod switch
	{
        TimePeriod.Minute => TimeSpan.FromMinutes(1),
        TimePeriod.FiveMinutes => TimeSpan.FromMinutes(5),
        TimePeriod.Day => TimeSpan.FromDays(1),
        TimePeriod.Week => TimeSpan.FromDays(7),
        TimePeriod.Month => TimeSpan.FromDays(30),
		_ => throw new NotSupportedException("Time period not supported.") 
    };

    private async Task<IDictionary<string, int>> ReturnSymbolMapper(IEnumerable<string> upcomingSymbols, List<Symbol> allSymbols, CancellationToken cancellationToken)
	{
		upcomingSymbols = upcomingSymbols.Select(x => x.ToUpper());

        var symbolsInDb = allSymbols.Select(x => x.Name);
		var newSymbols = upcomingSymbols.Except(symbolsInDb).ToList();

		if (newSymbols.Any())
        {
            var unsupportedSymbols = newSymbols.Except(_supportedSymbols.Value);

            if (unsupportedSymbols.Any())
            {
                _logger.LogWarning("Unsupported symbols detected {UnsupportedSymbols}. They will be ommitted.", unsupportedSymbols);
                newSymbols.RemoveAll(x => unsupportedSymbols.Contains(x));
            }

			var newEtries = newSymbols.Select(x => new Symbol() { IntCode = (int)Enum.Parse<Symbols>(x), Name = x });
			var updatedEntries = new List<Symbol>();

			foreach (var entry in newEtries)
            {
                updatedEntries.Add(_symbolRepository.Add(entry));
            }

			await _symbolRepository.SaveChanges(cancellationToken).ConfigureAwait(false);

			allSymbols.AddRange(updatedEntries);
        }

		return allSymbols.ToDictionary(x => x.Name, x => x.Id);
    }
}
