using Binance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance.Domain.Interfaces.Repositories;

public interface ISymbolPriceRepository : IBaseRepository<SymbolPrice>
{
}
