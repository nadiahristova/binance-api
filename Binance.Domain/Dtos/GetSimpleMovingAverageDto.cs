using Binance.Domain.Enums;

namespace Binance.Domain.Dtos;

/// <summary>
/// 
/// </summary>
/// <param name="n"></param>
/// <param name="p">booo</param>
/// <param name="s"></param>
public record GetSimpleMovingAverageDto(int n, TimePeriod p, DateTime? s);
