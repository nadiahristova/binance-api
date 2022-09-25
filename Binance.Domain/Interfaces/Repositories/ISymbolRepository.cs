using Binance.Domain.Entities;

namespace Binance.Domain.Interfaces.Repositories;

public interface ISymbolRepository : IBaseRepository<Symbol>
{
    Task<IEnumerable<decimal>> GetAllPricesBySymbol(string symbol, DateTime startDateUtc, CancellationToken cancellationToken);
}