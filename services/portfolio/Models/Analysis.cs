using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace portfolio.Models
{
    public class Analysis
    {
        public int Id { get; set; }
        public string StockSymbol {get; set;}
        public string DateCreated {get; set;}
        public string Content {get; set;}
        public string UserId { get; set; }
        public int StockId {get; set;}
        public Stock Stock {get; set;}
    }
}