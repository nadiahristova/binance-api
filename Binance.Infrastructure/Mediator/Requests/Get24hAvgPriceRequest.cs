using MediatR;

namespace Binance.Infrastructure.Mediator.Requests;

public record Get24hAvgPriceRequest(string Symbol) : IRequest<decimal>;