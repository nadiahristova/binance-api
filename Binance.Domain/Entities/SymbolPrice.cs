using Binance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance.Domain.Entities
{
    public class SymbolPrice : IEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime TradeTime { get; set; }

        public decimal Price { get; set; }

        public int SymbolId { get; set; }
        public virtual Symbol Symbol { get; set; }
    }
}
