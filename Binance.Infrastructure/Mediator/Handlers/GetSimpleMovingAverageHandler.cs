using Binance.Domain.Dtos;
using Binance.Domain.Interfaces;
using Binance.Infrastructure.Mediator.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Binance.Infrastructure.Mediator.Handlers;

public class GetSimpleMovingAverageHandler : IRequestHandler<GetSimpleMovingAverageRequest, decimal>
{
    private readonly ISymbolPriceService _symbolPriceService;
    private readonly ILogger<Get24hAvgPriceHandler> _logger;

    public GetSimpleMovingAverageHandler(
        ISymbolPriceService symbolPriceService,
        ILogger<Get24hAvgPriceHandler> logger)
    {
        _symbolPriceService = symbolPriceService;
        _logger = logger;
    }

    public async Task<decimal> Handle(GetSimpleMovingAverageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Requested average price for symbol {Symbol}.", request.Symbol);

        return await _symbolPriceService.GetSimpleMovingAverage(request.Symbol, request.DataPoints, request.TimePeriod, request.StartDate, cancellationToken).ConfigureAwait(false);
    }
}