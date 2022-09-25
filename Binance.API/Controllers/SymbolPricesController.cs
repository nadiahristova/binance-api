using Binance.Domain.Dtos;
using Binance.Infrastructure.Mediator.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Binance.API.Controllers;

[ApiController]
[Route("api/")]
public class SymbolPricesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SymbolPricesController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Retrieves average price for symbol for the past 24 hours
    /// </summary>
    /// <param name="symbolWrapper"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Average price</returns>
    [ResponseCache(Duration = 60)]
    [HttpGet("{Symbol}/24hAvgPrice")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
    [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<decimal>> Get24hAvgPrice([FromRoute] SymbolWrapperDto symbolWrapper, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new Get24hAvgPriceRequest(symbolWrapper.Symbol), cancellationToken).ConfigureAwait(false);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves average price value for period of time
    /// </summary>
    /// <returns>Average price</returns>
    [HttpGet("{Symbol}/SimpleMovingAverage")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
    [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<decimal>> GetSimpleMovingAverage([FromRoute] SymbolWrapperDto symbolWrapper, [FromQuery] GetSimpleMovingAverageDto queryParams, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSimpleMovingAverageRequest(symbolWrapper.Symbol, queryParams.n, queryParams.p, queryParams.s), cancellationToken).ConfigureAwait(false);

        return Ok(result);
    }
}