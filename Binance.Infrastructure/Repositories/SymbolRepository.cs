using Binance.Domain.Entities;
using Binance.Domain.Exceptions.Custom;
using Binance.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Binance.Infrastructure.Repositories;
public class SymbolRepository : BaseDbRepository<Symbol, BinanceContext>, ISymbolRepository
{
    public SymbolRepository(BinanceContext currentContext) : base(currentContext)
    {
    }

    public async Task<IEnumerable<decimal>> GetAllPricesBySymbol(string symbol, DateTime startDateUtc, CancellationToken cancellationToken)
    {
        var symbolExists = await _dbSet.AnyAsync(x => x.Name == symbol, cancellationToken).ConfigureAwait(false);

        if (!symbolExists)
        {
            throw new NotFoundDomainException($"Symbol {symbol} not found in the database.");
        }

        return await _dbSet.AsNoTracking()
            .Where(x => x.Name == symbol)
            .Include(x => x.SymbolPrices)
            .SelectMany(s => s.SymbolPrices)
            .Where(x => x.TradeTime >= startDateUtc)
            .Select(x => x.Price)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}