using System.ComponentModel;

namespace Binance.Domain.Enums;

public enum TimePeriod
{
    [Description("1w")]
    Week = 1,
    [Description("1d")]
    Day,
    [Description("30m")]
    Month,
    [Description("5m")]
    FiveMinutes,
    [Description("1m")]
    Minute
}
