using Binance.Domain.Interfaces;

namespace Binance.Domain.Entities;

public class Symbol : IEntity
{
    public int Id { get; set; }
    public int IntCode { get; set; }
    public string Name { get; set; }

    public virtual IEnumerable<SymbolPrice> SymbolPrices { get; set; }
}
