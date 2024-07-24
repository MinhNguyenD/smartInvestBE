using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace portfolio.Models
{
    [Table("portfolios")]
    public class Portfolio
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public ICollection<Holding> Holdings { get; set; }
    }
}