using Binance.Domain.Dtos;
using Binance.Domain.Enums;
using MediatR;

namespace Binance.Infrastructure.Mediator.Requests;

public record GetSimpleMovingAverageRequest(string Symbol, int DataPoints, TimePeriod TimePeriod, DateTime? StartDate) : IRequest<decimal>;
