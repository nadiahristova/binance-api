using Binance.Domain.Interfaces;
using Binance.Infrastructure.Mediator.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Binance.Infrastructure.Mediator.Handlers;

public class Get24hAvgPriceHandler : IRequestHandler<Get24hAvgPriceRequest, decimal>
{
    private readonly ISymbolPriceService _symbolPriceService;
    private readonly ILogger<Get24hAvgPriceHandler> _logger;

    public Get24hAvgPriceHandler(
        ISymbolPriceService symbolPriceService,
        ILogger<Get24hAvgPriceHandler> logger)
    {
        _symbolPriceService = symbolPriceService;
        _logger = logger;
    }

    public async Task<decimal> Handle(Get24hAvgPriceRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Requested 24h average price for symbol {Symbol}.", request.Symbol);

        return await _symbolPriceService.Get24hAvgPrice(request.Symbol, cancellationToken).ConfigureAwait(false);
    }
}