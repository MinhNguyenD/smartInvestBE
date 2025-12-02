using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio.Models
{
    [Table("holdings")]
    public class Holding
    {
        public int PortfolioId { get; set; }
        public Portfolio Portfolio { get; set; } 
        public int StockId { get; set; }
        public Stock Stock { get; set; }
    }
}