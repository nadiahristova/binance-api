using Binance.Domain.Entities;
using Binance.Domain.Interfaces.Repositories;

namespace Binance.Infrastructure.Repositories;
public class SymbolPriceRepository : BaseDbRepository<SymbolPrice, BinanceContext>, ISymbolPriceRepository
{
    public SymbolPriceRepository(BinanceContext currentContext) : base(currentContext)
    {
    }
}
