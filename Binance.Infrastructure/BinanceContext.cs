using Binance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Binance.Infrastructure;

public class BinanceContext : DbContext
{
    public BinanceContext()
    {
    }

    public BinanceContext(DbContextOptions<BinanceContext> options) : base(options)
    {
    }

    public DbSet<Symbol> Symbols { get; set; }

    public DbSet<SymbolPrice> SymbolPrices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
